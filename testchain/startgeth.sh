rm -rf devChain/chainData
rm -rf devChain/dapp
rm -rf devChain/nodes
rm -rf devchain/nodekey

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
#& $scriptDir\IndexSymbols.ps1

$scriptDir\geth.exe --datadir=devChain init genesis_clique.json
$scriptDir\geth.exe --nodiscover --rpc --datadir=devChain  --rpccorsdomain "*" --mine --rpcapi "eth,web3,personal,net,miner,admin,debug" --unlock 0x12890d2cce102216644c59dae5baed380d84830c --password "pass.txt" --verbosity 0 console  