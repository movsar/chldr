let mediaRecorder;
let recordedBlob;

let latestRecordingId;
export function addExistingEntryRecording(soundDto) {
    console.log(soundDto);

    let audioContainer = document.getElementById("audioContainer");
    if (!audioContainer) {
        console.error("audio container is null");
        return;
    }

    let audioUrl = URL.createObjectURL(base64ToBlob(soundDto.recordingB64));
    let recordedAudio = createAudioElement(audioUrl, soundDto.soundId);
    let removeButton = createRemoveButton(soundDto.soundId);


    audioContainer.appendChild(recordedAudio);
    audioContainer.appendChild(removeButton);
}

function base64ToBlob(base64String) {
    const byteCharacters = atob(base64String);
    const byteNumbers = new Array(byteCharacters.length);

    for (let i = 0; i < byteCharacters.length; i++) {
        byteNumbers[i] = byteCharacters.charCodeAt(i);
    }

    const byteArray = new Uint8Array(byteNumbers);

    return new Blob([byteArray], { type: "audio/ogg; codecs=opus" });
}

export function startRecording(recordingId) {
    console.log(recordingId);
    showStopButton();

    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {

            latestRecordingId = recordingId;

            let chunks = [];

            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.audioBitsPerSecond
            mediaRecorder.addEventListener("dataavailable", function (event) {
                chunks.push(event.data);
            });

            mediaRecorder.addEventListener("stop", function () {
                const audioBlob = new Blob(chunks, { type: "audio/ogg; codecs=opus" });
                recordedBlob = audioBlob;

                const audioUrl = URL.createObjectURL(audioBlob);
                const recordedAudio = createAudioElement(audioUrl, recordingId);

                let removeButton = createRemoveButton(recordingId);

                const audioContainer = document.getElementById("audioContainer");
                audioContainer.appendChild(recordedAudio);
                audioContainer.appendChild(removeButton);
            });

            mediaRecorder.start();
        })
        .catch(function (err) {
            console.log("The following error occurred: " + err);
        });
}

export function stopRecording() {
    if (!mediaRecorder) {
        return;
    }

    showStartButton();

    return new Promise((resolve, reject) => {
        mediaRecorder.addEventListener("stop", function () {
            const reader = new FileReader();
            reader.onloadend = function () {
                const base64String = reader.result.split(',')[1];
                resolve({ id: latestRecordingId, data: base64String });
            };
            reader.readAsDataURL(recordedBlob);
        });
        mediaRecorder.stop();
    });
}

export function showStopButton() {
    const recordButton = document.getElementById("recordButton");

    recordButton.textContent = "Stop recording";
    recordButton.classList.remove("record");
    recordButton.classList.add("stop");
}

export function showStartButton() {
    const recordButton = document.getElementById("recordButton");

    recordButton.textContent = "Add pronunciation";
    recordButton.classList.remove("stop");
    recordButton.classList.add("record");
}

export function createAudioElement(audioUrl, recordingId) {
    const recordedAudio = document.createElement("audio");
    recordedAudio.src = audioUrl;
    recordedAudio.controls = true;
    recordedAudio.id = `recordedAudio_${recordingId}`;

    return recordedAudio;
}

export function createRemoveButton(recordingId) {
    var button = document.createElement('button');
    button.classList.add('btn');
    button.classList.add('btn-danger');
    button.textContent = 'Remove';
    button.addEventListener('click', () => {
        const audioToRemove = document.getElementById(`recordedAudio_${recordingId}`);
        audioToRemove.remove();
        button.remove();

        DotNet.invokeMethodAsync("chldr_shared", "WordEdit_RemoveSound_ClickHandler", recordingId);
    });

    return button;
}


export function enable(selector) {
    var container = document.querySelector(selector);
    if (container == null) {
        return;
    }
    var elements = container.getElementsByTagName("*");

    for (var i = 0; i < elements.length; i++) {
        elements[i].disabled = false;
    }
}
export function disable(selector) {
    var container = document.querySelector(selector);
    if (container == null) {
        return;
    }
    var elements = container.getElementsByTagName("*");

    for (var i = 0; i < elements.length; i++) {
        elements[i].disabled = true;
    }
}

export function showConfirmationDialog() {
    // Пример использования:
    openModal();
}

export function showPrompt(message) {
    return prompt(message, 'Type anything here');
}

export function clickShowRandoms() {
    try {
        const btnShowRandoms = document.querySelector(".chldr_btn_show-randoms");
        btnShowRandoms.click(), 3000
    } catch (ex) {
        console.log("couldn't click it :(");
    }
}