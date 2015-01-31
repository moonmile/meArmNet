all:testapp.exe app1.exe app2.exe

testapp.exe: I2CNet.dll libi2cnet.so Program.cs
	gmcs /out:testapp.exe -sdk:4.5 /r:I2CNet.dll Program.cs 
app1.exe: I2CNet.dll libi2cnet.so app1.cs
	gmcs /out:app1.exe -sdk:4.5 /r:I2CNet.dll app1.cs 
app2.exe: I2CNet.dll libi2cnet.so app2.cs
	gmcs /out:app2.exe -sdk:4.5 /r:I2CNet.dll app2.cs 
I2CNet.dll: libi2cnet.so I2CNet.cs
I2CNet.dll: libi2cnet.so I2CNet.cs
	gmcs /target:library /out:I2CNet.dll -sdk:4.5 I2CNet.cs
libi2cnet.so: i2cnet.cpp
	g++ -shared -o libi2cnet.so i2cnet.cpp

