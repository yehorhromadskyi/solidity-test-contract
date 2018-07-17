pragma solidity ^0.4.22;

contract Bravo {
    
    struct Participant {
        uint256 balance;
        bool isActive;
    }

    string public _symbol;
    string public  _name;
    uint8 public _decimals = 18;
    uint _totalSupply;
    
    mapping (address => Participant) participants;

    event Transfer(address indexed from, address indexed to, uint amount);

    constructor () public {
        _symbol = "BRV";
        _name = "Bravo";
        _totalSupply = 1000*1000**uint(_decimals);
    }

    function () public payable {
        if (isActiveParticipant(msg.sender)) {
            participants[msg.sender].balance += msg.value;
        }
        else {
            participants[msg.sender].isActive = true;
            participants[msg.sender].balance = msg.value;
        }
    }

    function transfer(address to, uint256 amount) public {
        require(isActiveParticipant(msg.sender));
        require(isActiveParticipant(to));
        require(participants[msg.sender].balance >= amount);
        require(participants[to].balance + amount >= participants[to].balance);

        participants[msg.sender].balance -= amount;
        participants[to].balance += amount;

        emit Transfer(msg.sender, to, amount);
    }

    function isActiveParticipant(address a) private view returns(bool) {
        return participants[a].isActive;
    }
}