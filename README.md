Campfire.Api
============

This is a simple C# implementation in Windows RT 8.1 of the Campfire API. I developed this for use in my StovePipe Campfire app for Windows 8.1, and extracted the library from that project so that others can integrate Campfire support into their Windows RT projects. 

This implementation uses OAuth and not username/password, so you'll need to implement a web-based redirect and success URL setup with your Campfire secret / key. I plan to release my rather simple implementation of a web project to handle OAuth on GitHub as well once I scrub the personal info from my private repo.
