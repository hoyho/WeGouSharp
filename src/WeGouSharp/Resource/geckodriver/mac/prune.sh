echo cleaning unmanaged resources ...

#kill all geckodriver
ps -ef | grep "geckodriver" | grep "port"| awk {'print "kill " $2'} | bash   > /dev/null 2>&1 &

#kill unused firefox
ps -ef | grep "firefox" | grep  "marionette"  | awk {'print "kill " $2'} | bash  > /dev/null 2>&1 &

exit
