FROM ubuntu:22.04

ARG TARGET_ARCH

LABEL TargetArchitecture=${TARGET_ARCH}

COPY ${TARGET_ARCH}/setup.sh /setup.sh
RUN chmod +x /setup.sh
RUN /setup.sh

ENV TARGET_ARCH="${TARGET_ARCH}"
ENV SRC_DIR="/src"

CMD "${SRC_DIR}/build/linux/build.sh" "${TARGET_ARCH}"