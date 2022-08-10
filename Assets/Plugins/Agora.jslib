mergeInto(LibraryManager.library, {
    JoinRTC: function (appId, channel, token, uid) {
        joinRTC(UTF8ToString(appId), UTF8ToString(channel), UTF8ToString(token), UTF8ToString(uid));
    },
    SetVolume: function (uid, volume) {
        const track = remoteTracks[UTF8ToString(uid)];
        if(track) {
            track.setVolume(volume);
        }
    }
});