
let localTrack;
let remoteTracks = {};

const client = AgoraRTC.createClient({ mode: "rtc", codec: "vp8" });

client.on("user-published", async (user, mediaType) => {
    await client.subscribe(user, mediaType);
    console.log("subscribe success");

    if (mediaType === "audio") {
        const remoteAudioTrack = user.audioTrack;
        remoteAudioTrack.play();
        remoteAudioTrack.setVolume(0);
        remoteTracks[user.uid] = remoteAudioTrack;
    }

    client.on("user-unpublished", async (user) => {
        await client.unsubscribe(user);
    });
});

async function joinRTC(appId, channel, token, uid) {
    await client.join(appId, channel, token, uid);
    localTrack = await AgoraRTC.createMicrophoneAudioTrack();
    await client.publish([localTrack]);
    console.log("publish success!");
}

async function setMuted(muted) {
    localTrack.setMuted(muted);
}