pragma solidity ^0.4.22;

contract Sqrt {
    function sqrt(uint256 number) public pure returns(uint256) {
        return number * number;
    }
}