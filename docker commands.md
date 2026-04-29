** Docker Commands: **

1 - docker compose up

2 - docker compose down

3 - docker compose up --build

4 - docker compose build --no-cache service/dll-name

5 - docker run --rm -v "%cd%":/work -w /work alpine/openssl ^                  // run command for docker generate certificate
    req -x509 -nodes -days 825 -newkey rsa:2048 ^
    -keyout id-local.key -out id-local.crt ^
    -config id-local.conf -extensions v3_ca -sha256 -batch

6 - docker run --rm -v "%cd%":/work -w /work alpine/openssl                   // run command to create id-local.pfx file
    pkcs12 -export -out id-local.pfx -inkey id-local.key 
    -in id-local.crt -password pass:YourPassword123
