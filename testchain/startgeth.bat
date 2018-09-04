RD /S /Q %~dp0\devChain\geth\chainData
RD /S /Q %~dp0\devChain\geth\dapp
RD /S /Q %~dp0\devChain\geth\nodes
del %~dp0\devchain\geth\nodekey

geth.exe  --datadir=devChain init genesis_clique.json
geth.exe --mine --rpc --ws --networkid=39318 --cache=2048 --maxpeers=0 --datadir=devChain  --ipcpath "geth.ipc"  --rpccorsdomain "*" --rpcapi "eth,web3,personal,net,miner,admin,debug" --unlock 0x12890d2cce102216644c59dae5baed380d84830c --password "pass.txt" --verbosity 0 console  