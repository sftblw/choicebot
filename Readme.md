# choicebot for mastodon

Simple choicbot for mastodon with dotnet core, C#.

간단한 선택봇입니다.

see in action: <a href="https://twingyeo.kr/@choicebot" rel="me">@choicebot on @twingyeo.kr</a>

warning: This bot still has many hard-coded string values. use with caution.

## installation

### 1. build and first time run

1. install .net core 2.1 or above (arch: `pacman -S dotnet-sdk`)
2. cd to `./choicebot`
3. build: `dotnet build --output ./serve -c Release`
4. cd to `./serve`, first run: `dotnet ./choicebot.dll` (this time there's no ID/PW login)

### 2. as a systemd Service

1. modify `./choicebot/choicebot.service` for your needs. (recommended to copy to your place)
2. maybe `sudo cp choicebot.service /etc/systemd/system/` ?
3. `sudo systemctl enable choicebot`