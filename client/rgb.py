import socket
import RPi.GPIO as GPIO
import sys
import os

mysock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

GPIO.setmode(GPIO.BOARD)
GPIO.setup(7, GPIO.OUT)
GPIO.setup(11, GPIO.OUT)
GPIO.setup(13, GPIO.OUT)
GPIO.output(7, GPIO.LOW)
GPIO.output(11, GPIO.LOW)
GPIO.output(13, GPIO.LOW)

try:
    mysock.bind(("192.168.1.111", 12345))
except socket.error:
    print("Failed to bind")
    sys.exit()
mysock.listen(5)
conn, addr = mysock.accept()
print ('Connected by ', addr)

try:
    while True:
        data = conn.recv(1024)
        if not data:
            break
        if data =="RED":
            print ('RED')
            GPIO.output(7, GPIO.HIGH)
	    GPIO.output(11, GPIO.LOW)
	    GPIO.output(13, GPIO.LOW)
        if data =="BLUE":
            print ('BLUE')
            GPIO.output(7, GPIO.LOW)
	    GPIO.output(11, GPIO.HIGH)
	    GPIO.output(13, GPIO.LOW)
        if data =="GREEN":
	    print ('GREEN')
	    GPIO.output(7, GPIO.LOW)
	    GPIO.output(11, GPIO.LOW)
	    GPIO.output(13, GPIO.HIGH)
        conn.sendall(data)
except socket.error:
    print ('Remote client disconnected')

GPIO.cleanup()
conn.close()
mysock.close()
