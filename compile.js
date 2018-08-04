const fs = require('fs');
const solc = require('solc');

var input = {
	'Bravo': fs.readFileSync('./contracts/Bravo.sol', 'UTF-8')
}

var output = solc.compile({sources: input}, 1)

for (var contractName in output.contracts)
{
	var validFileName = contractName.replace(':', '_');
	
	fs.writeFileSync('bin/' + validFileName + '.bin', output.contracts[contractName].bytecode);
	fs.writeFileSync('bin/' + validFileName + '.abi', output.contracts[contractName].interface);

	//console.log('Writing ' + validFileName);
}