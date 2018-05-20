FROM microsoft/dotnet 

ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE true
ENV DOTNET_CLI_TELEMETRY_OPTOUT true

RUN groupadd -g 1000 jenkins && \
    useradd -u 1000 -g 1000 -m -s /bin/bash jenkins

USER jenkins