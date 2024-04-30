"# Audio Recorder" This DLL wraps NAudio NuGet Package. It is used to make recording speaker (system) audio and mic audio simpler. It is made in Visual Studio 2022, with C#

# A. Namespace
The namespace of this DLL is NAudio_Wrapper

# B. Classes
This DLL consists of 3 classes:
- Class_Speaker is used to record sound from speaker
- Class_Mic is used to record sound from mic
- Class_SpeakerAndMic is used to record sound from both speaker and mic. It calls functions from the other 2 classes.

# C. How to Use
Every single property and function from Class_Speaker and Class_Mic can be used in the exact same way. For example, Class_Speaker has the function StartRecording() that accepts 0 argument. This means Class_Mic also has the function StartRecording() that accepts 0 argument.

## 1. Starting and Stopping Recording
### a. Starting Recording
If we want to record speaker or mic sound individually, simply call:
```C#
NAudio_Wrapper.Class_Speaker.StartRecording();
NAudio_Wrapper.Class_Mic.StartRecording();
```
These functions return false if there's an error, such as if mic is not found.


If we want to record both at once, call:
```C#
NAudio_Wrapper.Class_SpeakerAndMic.StartRecording();
```
This function simply calls StartRecording() from Class_Speaker and Class_Mic

### b. Stopping Recording
To stop recording, use these functions:
```C#
NAudio_Wrapper.Class_Speaker.StopRecording(wavFilePath);
NAudio_Wrapper.Class_Mic.StopRecording(wavFilePath);
NAudio_Wrapper.Class_SpeakerAndMic.StopRecording(wavFilePath);
```
Upon stopping recording, a WAV file is created. wavFilePath is the destination path of that WAV.

Make sure to use the right class. If we started recording with Class_Speaker and Class_Mic, we should also stop recording both of them individually (meaning, don't use Class_SpeakerAndMic to stop recording).

If we used Class_SpeakerAndMic to start and stop recording, what happens is that 2 temporary wav files (each 1 for speaker and mic) are created. These 2 files and then mixed into 1 with wavFilePath as the destination path.

## 2. Pausing and Muting
To pause/mute the recording, use the IsPaused and IsMuted properties from Class_Speaker and Class_Mic:
```C#
NAudio_Wrapper.Class_Speaker.IsPaused = false;
NAudio_Wrapper.Class_Speaker.IsMuted = true;

NAudio_Wrapper.Class_Mic.IsPaused = false;
NAudio_Wrapper.Class_Mic.IsMuted = true;
```

## 3. IsRecording
To know if speaker or mic is being recorded, we can use the IsRecording property
```C#
if (NAudio_Wrapper.Class_Speaker.IsRecording)
    ...

if (NAudio_Wrapper.Class_Mic.IsRecording)
    ...
```
Unlike IsPaused and IsMuted that can be set, IsRecording is private. So, codes from different class can't set its value.