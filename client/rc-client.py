#!/usr/bin/python
import lirc
import time
import socket

HOST, PORT = "192.168.1.4", 12345
 
sockid = lirc.init("rcled", blocking=False)
mysock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)

try:
    mysock.connect((HOST, PORT)) 
except socket.error:
    print("Failed to connect")
try:
    while True:
       try:
          button = lirc.nextcode()
          if len(button) == 0: continue
          mysock.sendall(bytes(button[0]))
          print (button[0])
          time.sleep(1)
       except KeyboardInterrupt:
          lirc.deinit()
          mysock.close()
          break
except socket.error:
    print ("Remote client disconnected")
    mysock.close()
