#ifndef CMIXR_H
#define CMIXR_H

#include <stdarg.h>
#include <stdbool.h>
#include <stdint.h>
#include <stdlib.h>

typedef enum {
  AUDIO_RESULT_OK,
  AUDIO_RESULT_INVALID_BUFFER,
  AUDIO_RESULT_INVALID_CHANNEL,
  AUDIO_RESULT_NO_CHANNELS,
} AudioResult;

typedef enum {
  INTERPOLATION_TYPE_NONE,
  INTERPOLATION_TYPE_LINEAR,
} InterpolationType;

typedef struct AudioSystem AudioSystem;

typedef struct {
  uint8_t channels;
  int32_t sampleRate;
  uint8_t bitsPerSample;
  bool floatingPoint;
} AudioFormat;

typedef struct {
  double volume;
  double speed;
  double panning;
  bool looping;
  InterpolationType interpolationType;
  int32_t loopStart;
  int32_t loopEnd;
} ChannelProperties;

typedef struct {
  uint8_t *data;
  uintptr_t dataLength;
  AudioFormat format;
} CPCM;

#ifdef __cplusplus
extern "C" {
#endif // __cplusplus

AudioSystem *mxCreateSystem(int32_t sample_rate, uint16_t channels);

void mxDeleteSystem(AudioSystem *system);

void mxSetBufferFinishedCallback(AudioSystem *system, void (*callback)(uint16_t, int32_t));

int32_t mxCreateBuffer(AudioSystem *system);

AudioResult mxDeleteBuffer(AudioSystem *system, int32_t buffer);

AudioResult mxUpdateBuffer(AudioSystem *system,
                           int32_t buffer,
                           const void *data,
                           uintptr_t data_length,
                           AudioFormat format);

AudioResult mxPlayBuffer(AudioSystem *system,
                         int32_t buffer,
                         uint16_t channel,
                         ChannelProperties properties);

AudioResult mxQueueBuffer(AudioSystem *system, int32_t buffer, uint16_t channel);

AudioResult mxSetChannelProperties(AudioSystem *system,
                                   uint16_t channel,
                                   ChannelProperties properties);

AudioResult mxPlay(AudioSystem *system, uint16_t channel);

AudioResult mxPause(AudioSystem *system, uint16_t channel);

AudioResult mxStop(AudioSystem *system, uint16_t channel);

float mxAdvance(AudioSystem *system);

uint16_t mxGetNumChannels(AudioSystem *system);

bool mxIsPlaying(AudioSystem *system, uint16_t channel);

uint16_t mxGetAvailableChannel(AudioSystem *system);

CPCM *mxPCMLoadWav(const uint8_t *data, uintptr_t data_length);

void mxPCMFree(CPCM *pcm);

#ifdef __cplusplus
} // extern "C"
#endif // __cplusplus

#endif /* CMIXR_H */
