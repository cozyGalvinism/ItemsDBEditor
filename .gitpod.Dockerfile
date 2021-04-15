FROM gitpod/workspace-full
RUN sudo apt-get update \
    && sudo apt-get install -y dotnet-sdk-3.1 \
    && sudo rm -rf /var/lib/apt/lists/*