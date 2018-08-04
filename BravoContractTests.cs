using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Nethereum.Geth;
using Nethereum.Hex.HexTypes;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Web3.Accounts.Managed;
using Newtonsoft.Json;
using Xunit;

namespace NethereumTest
{
    public class BravoContractTests
    {
        const string Password = "password";

        const string Sender = "0x12890d2cce102216644c59dae5baed380d84830c";
        const string Receiver = "0xd147520dba4367a8d243b7feb69f5161c0177de4";

        Contract _contract;

        public BravoContractTests()
        {
            var currentDirectory = Environment.CurrentDirectory;
            while (!currentDirectory.EndsWith("bin"))
            {
                currentDirectory = Directory.GetParent(currentDirectory).FullName;
            }
            var abif = Directory.GetFiles(currentDirectory).First(f => f.EndsWith("Bravo.abi"));
            var binf = Directory.GetFiles(currentDirectory).First(f => f.Contains("Bravo.bin"));

            _contract = new Contract
            {
                Abi = File.ReadAllText(abif),
                Bytecode = File.ReadAllText(binf)
            };
        }

        [Fact]
        public async Task DeployContractTest() 
        {
            var web3Geth = new Web3Geth(new ManagedAccount(Sender, Password));

            // var accounts = await web3Geth.Eth.Accounts.SendRequestAsync();
            // if (accounts.Length == 0)
            // {
            //     await web3Geth.Personal.NewAccount.SendRequestAsync(Password);
            //     accounts = await web3Geth.Eth.Accounts.SendRequestAsync();
            // }

            // var senderAddress = accounts.First();


            // var unlockAccountResult = await web3Geth.Personal.UnlockAccount.SendRequestAsync(senderAddress, Password, 120);
            // Assert.True(unlockAccountResult);

            //var b = await web3Geth.Eth.GetBalance.SendRequestAsync(senderAddress);

            var receipt = await web3Geth.Eth.DeployContract.SendRequestAndWaitForReceiptAsync(
                _contract.Abi, _contract.Bytecode, Sender, new HexBigInteger(900000));

            // var mineResult = await web3Geth.Miner.Start.SendRequestAsync();
            // Assert.False(mineResult);

            // TransactionReceipt receipt = null;

            // do
            // {
            //     await Task.Delay(5000);
            //     receipt = await web3Geth.Eth.Transactions.GetTransactionReceipt.SendRequestAsync(transactionHash);
            // } while (receipt == null);

            // mineResult = await web3Geth.Miner.Stop.SendRequestAsync();
            // Assert.True(mineResult);

            var contractAddress = receipt.ContractAddress;
            var contract = web3Geth.Eth.GetContract(_contract.Abi, contractAddress);
            var balanceOfFunction = contract.GetFunction("balanceOf");

            //var balance = await balanceOfFunction.CallAsync<int>(5);
            Assert.True(balanceOfFunction != null);
        }

        [Fact]
        public void ContractIsNotNull() {
            Assert.NotNull(_contract);
            Assert.NotNull(_contract.Abi);
            Assert.NotNull(_contract.Bytecode);
        }
    }
}