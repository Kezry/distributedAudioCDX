# distributedAudioCDX Virtual Audio Driver

This directory contains the first WDK-buildable driver skeleton for the virtual audio endpoint workstream.

The current target is CI build validation only:

- It is not a signed production driver.
- It does not yet expose a full SysVAD render endpoint.
- It gives the repository a real WDK project path that can later be replaced with the full SysVAD-derived miniport implementation.

Production installation requires a proper audio miniport implementation, driver signing, and the Microsoft hardware signing flow.

