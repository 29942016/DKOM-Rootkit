# ckit
This application finds the handle to the task managers 
process chain in memory and attempts to modify it. The application
is also packed with other functions to manipulation the task manager.

- Disable redraw
- Disable process status refresh
- Delete process
- Some other junk, check sTaskManager.cls for functions.

original dkom methodology:
http://forums.codeguru.com/showthread.php?406555-How-to-hide-your-program-from-the-Task-Manager&p=1492556#post1492556

I've just translated the previously mentioned code snippet from VB/Windows XPx32 -> C#/Windows 10 x64 by
updating the necessary required winapi calls to find the tasklist in the windows 10 environment.

<b>tldr;</b> Hides processes from the window's task manager.
