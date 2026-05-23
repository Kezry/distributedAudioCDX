# Audio Packet V1

All integer fields are little-endian.

| Offset | Size | Field |
|---:|---:|---|
| 0 | 2 | magic, `0xCDAC` |
| 2 | 2 | version, `1` |
| 4 | 4 | stream_id |
| 8 | 4 | sequence |
| 12 | 4 | sample_rate |
| 16 | 4 | channel_layout |
| 20 | 4 | frame_count |
| 24 | 8 | capture_timestamp_us |
| 32 | 8 | play_at_timestamp_us |
| 40 | 4 | reserved |
| 44 | n | PCM payload |

The first implementation sends 48 kHz PCM S16LE mono packets to each selected endpoint after Windows-side channel extraction.

