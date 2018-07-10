using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Newtonsoft.Json;
using Xunit;

namespace NethereumTest
{
    public class SqrtContractTests
    {
        Contract _contract;

        public SqrtContractTests()
        {
            var currentDirectory = Environment.CurrentDirectory;
            while (!currentDirectory.EndsWith("bin"))
            {
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }

            var abif = Directory.GetFiles(currentDirectory).First(f => f.EndsWith("abi"));
            var binf = Directory.GetFiles(currentDirectory).First(f => f.EndsWith("bin"));

            _contract = new Contract
            {
                Abi = File.ReadAllText(abif),
                Bytecode = File.ReadAllText(binf)
            };
        }

        [Fact]
        public async Task DeployContractTest() {
            var senderAddress = "0x12890d2cce102216644c59daE5baed380d84830c";
            var password = "password";
            var web3Geth = new Web3Geth();

            var unlockAccountResult = await web3Geth.Personal.UnlockAccount.SendRequestAsync(senderAddress, password, 120);
            Assert.True(unlockAccountResult);

            var transactionHash = await web3Geth.Eth.DeployContract.SendRequestAsync(_contract.Abi, _contract.Bytecode, senderAddress, new HexBigInteger(290000));

            var mineResult = await web3Geth.Miner.Start.SendRequestAsync(6);
            Assert.False(mineResult);

            TransactionReceipt receipt = null;

            do
            {
                await Task.Delay(5000);
                receipt = await web3Geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            } while (receipt == null);

            mineResult = await web3Geth.Miner.Stop.SendRequestAsync();
            Assert.True(mineResult);

            var contractAddress = receipt.ContractAddress;
            var contract = web3Geth.Eth.GetContract(_contract.Abi, contractAddress);
            var sqrt = contract.GetFunction("sqrt");

            var result = await sqrt.CallAsync<int>(11);
            Assert.Equal(121, result);
        }

        [Fact]
        public void ContractIsNotNull() {
            Assert.NotNull(_contract);
            Assert.NotNull(_contract.Abi);
            Assert.NotNull(_contract.Bytecode);
        }
    }
}