# choicebot for mastodon

Simple choicbot for mastodon with dotnet core, C#.

간단한 선택봇입니다.

see in action: <a href="https://twingyeo.kr/@choicebot" rel="me">@choicebot on @twingyeo.kr</a>

## installation

### 1. build and first time run

#### .net core

1. install .net core 2.1 or above (arch x86/64: `pacman -S dotnet-sdk`)
2. cd to `./choicebot.netcore`
3. build: `dotnet build --output ./serve -c Release`
4. cd to `./serve`, first run: `dotnet ./choicebot.netcore.dll` (this time there's no ID/PW login)

#### .net framework

no description here

### 2. as a systemd Service

1. modify `./choicebot.netcore/choicebot.service` for your needs. (recommended to copy to your place)
2. maybe `sudo cp choicebot.netcore.service /etc/systemd/system/` ?
3. `sudo systemctl enable choicebot.netcore`