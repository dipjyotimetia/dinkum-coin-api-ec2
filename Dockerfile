FROM microsoft/dotnet 

ENV DOTNET_SKIP_FIRST_TIME_EXPERIENCE true
ENV DOTNET_CLI_TELEMETRY_OPTOUT true

RUN groupadd -g 497 jenkins && \
    useradd -u 497 -g 497 -m -s /bin/bash jenkins

USER jenkins