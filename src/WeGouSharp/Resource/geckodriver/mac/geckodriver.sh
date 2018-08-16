#! /bin/bash
echo "=====working in no log mode========"
echo "GECKO COMMANDLINE ARGS: " $@

mydir="$(dirname "${0}")"

pwd

#geckodriver --log fatal "$@" > /dev/null 2>&1 # works because everything is redirected to null
#geckodriver --log fatal "$@" &    # my testings shows the --log fatal doesn't do anything!

${mydir}/geckodriver "$@" > /dev/null 2>&1 &  # works without --log fatal, again all redirected to null
#geckodriver --log error "$@" &            # still get all DEBUGs, --log fatal doesn't work
#geckodriver -vvvv  "$@" &                # new -v option didn't work for me


pid="$!"

echo "geckodriver.sh pid is: $$"
echo "child geckodriver  pid is: $pid"

trap 'echo I am going down, so killing off my processes..; kill $pid; exit' SIGHUP SIGINT SIGQUIT SIGTERM

wait
