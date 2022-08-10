mergeInto(LibraryManager.library, {
  ConsoleLog: function (msg) {
    const logMsg = UTF8ToString(msg);
    console.log(`### From Unity: ${logMsg} ###`);
  },
  OpenPage: function (url) {
    const loc = UTF8ToString(url);
    window.open(loc);
  },
  GetRoomId: function () {
    const p = new URLSearchParams(location.search);
    const id = p.get('id');
    if(!id) {
      return -1;
    }
    else {
      return parseInt(id);
    }
  },
  PlayUrl: function (url) {
    const audioSrc = UTF8ToString(url);
    const audio = document.querySelector("audio");

    if (audio.canPlayType('application/vnd.apple.mpegurl')) {
      audio.src = audioSrc;
    } else if (Hls.isSupported()) {
      var hls = new Hls();
      hls.loadSource(audioSrc);
      hls.attachMedia(audio);
      hls.on(Hls.Events.MANIFEST_PARSED, (e, data) => {
        audio.play();
      });
    }
  },
  OpenMetaMask: function () {
    try{
      window.ethereum.enable();
    }
    catch {
      alert("Metamask extension not found");
    }
  },
  ToNextPage: function (museId) {
    window.location = window.location.origin + `?id=${museId}`;
  }
});