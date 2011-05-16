@echo off

set REL=Debug

copy "Application\bin\Console-%REL%\*.exe" TestBed
copy "Application\bin\Console-%REL%\*.dll" TestBed
copy "Application\bin\Console-%REL%\*.pdb" TestBed

copy "Application\bin\GUI-%REL%\*.exe" TestBed
copy "Application\bin\GUI-%REL%\*.dll" TestBed
copy "Application\bin\GUI-%REL%\*.pdb" TestBed

del "TestBed\*vshost*"

rem pause