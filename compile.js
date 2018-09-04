const fs = require('fs');
const solc = require('solc');

var input = {
	'SafeMath.sol': fs.readFileSync('./contracts/SafeMath.sol', 'UTF-8'),
	'ERC20Interface.sol': fs.readFileSync('./contracts/ERC20Interface.sol', 'UTF-8'),
	'Owned.sol': fs.readFileSync('./contracts/Owned.sol', 'UTF-8'),
	'Bravo.sol': fs.readFileSync('./contracts/Bravo.sol', 'UTF-8')
}

var output = solc.compile({sources: input}, 1)

for (var contractName in output.contracts)
{
	var validFileName = contractName.replace(':', '_');
	
	fs.writeFileSync('bin/' + validFileName + '.bin', output.contracts[contractName].bytecode);
	fs.writeFileSync('bin/' + validFileName + '.abi', output.contracts[contractName].interface);

	//console.log('Writing ' + validFileName);
}