# MusicPlayerMcp

`MusicPlayerMcp` is a simple **Model Context Protocol (MCP) server** that exposes a stdio-based interface for controlling local music playback. It is intended to be used by MCP‑aware AI assistants (such as GitHub Copilot) or any client that can speak the protocol over standard input/output.

The tool set is intentionally small and focused: play a track (by URL or search query) and stop playback.  There is no extra configuration or user interface; the server runs in the background and interacts via MCP messages.

---

## Available MCP Tools

All interactions happen through MCP requests on stdin/stdout.

### `playSong`
Requests playback of a song. The argument may be a full YouTube URL or a natural-language query (title/artist).

- **Input:** a single string containing the song query or URL.
- **Behavior:** resolves the query, downloads the audio, and begins local playback.
- **Response:** confirmation that playback has started, or an error message.

### `stopPlayback`
Stops any currently playing audio.

- **Input:** none.
- **Behavior:** halts playback immediately.
- **Response:** confirmation that playback was stopped.

> ⚠️ Only one track can play at a time; a new `playSong` request replaces any existing audio.

---

## Usage

1. Launch the server executable. It listens for MCP messages on its standard input and writes responses to standard output.
2. Send an MCP request with the desired tool name and parameters.
3. Read the response on stdout to know when the action completes.

From an MCP‑capable client (such as Copilot), the assistant simply invokes the tool by name. Example prompts:

- _"Play 'Bohemian Rhapsody' for me."_ → triggers `playSong` with query `Bohemian Rhapsody`.
- _"Stop the music."_ → triggers `stopPlayback`.

---

## Notes

- The server uses youtube.com as the source for audio; it does not require the user to install or configure a browser.
- It is a **stdio tool** and has no GUI.
- Designed for quick control via natural language; implementation details are intentionally hidden from users.

Enjoy seamless, AI‑driven local music playback!

