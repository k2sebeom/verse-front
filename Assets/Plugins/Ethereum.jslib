mergeInto(LibraryManager.library, {
  GetCurrentAddress: function () {
    let addr = "";
    if(window.ethereum && window.ethereum.selectedAddress) {
      addr = window.ethereum.selectedAddress;
    }
    var bufferSize = lengthBytesUTF8(addr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(addr, buffer, bufferSize);
    return buffer;
  },
  CheckNFTOwnership: async function (id) {
    if(!window.ethereum) {
      alert("Metamask extension not found");
      return;
    }
    if(!window.ethereum.selectedAddress) {
      try {
        await window.ethereum.enable();
      }
      catch {
        return;
      }
    }
    const p = new ethers.providers.JsonRpcProvider("https://polygon-rpc.com");
    const abi = ["function ownerOf(uint256 _tokenId) view returns(address)"];
    const c = new ethers.Contract("0x55C1e448843325B91A3d90745FB2c781400B93b9", abi, p);
    const owner = await c.ownerOf(id);
    SendMessage("GameManager", "HandleNFTResult", owner === window.ethereum.selectedAddress ? 1 : 0);
  }
});