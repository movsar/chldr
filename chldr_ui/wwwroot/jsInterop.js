// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.
let mediaRecorder;
let chunks = [];
let isRecording = false;
let recordedAudio;

export function toggleRecording() {
    if (!isRecording) {
        startRecording();
        document.getElementById("recordButton").textContent = "Stop";
        isRecording = true;
    } else {
        stopRecording();
        document.getElementById("recordButton").textContent = "Play";
        isRecording = false;
    }
}

export function startRecording() {
    navigator.mediaDevices.getUserMedia({ audio: true })
        .then(function (stream) {
            mediaRecorder = new MediaRecorder(stream);
            mediaRecorder.start();

            mediaRecorder.addEventListener("dataavailable", function (event) {
                chunks.push(event.data);
            });
        })
        .catch(function (err) {
            console.log("The following error occurred: " + err);
        });
}

export function stopRecording() {
    mediaRecorder.stop();
    mediaRecorder.addEventListener("stop", function () {
        const audioBlob = new Blob(chunks, { type: "audio/wav" });
        const audioUrl = URL.createObjectURL(audioBlob);
        recordedAudio = new Audio(audioUrl);
        recordedAudio.controls = true;
        document.getElementById("audioContainer").appendChild(recordedAudio);

        chunks = [];

        const formData = new FormData();
        formData.append("audio", audioBlob, "recorded_audio.wav");

        // Send the audio data to the server using fetch
        fetch("https://localhost:7065", {
            method: "POST",
            body: formData
        })
            .then(function (response) {
                // Handle the response from the server
                console.log("Audio sent successfully!");
            })
            .catch(function (error) {
                // Handle any error that occurred during the request
                console.log("Error sending audio:", error);
            });
    });
}

export function enable(selector) {
    var container = document.querySelector(selector);
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