// I2CNet 補助ライブラリ
// Ada I2C 16chサーボドライバ

#include <iostream>
#include <stdio.h>
#include <stdlib.h>
#include <stdint.h>
#include <math.h>
#include <linux/i2c-dev.h>
#include <fcntl.h>
#include <sys/ioctl.h>

#define LED0_ON_L 0x6
#define LED0_ON_H 0x7
#define LED0_OFF_L 0x8
#define LED0_OFF_H 0x9
extern "C"
{

int I2C_open( const char *devname, int mode ) {
	return open( devname, mode );
}
int I2C_ioctl( int fd, int mode, int addr ) {
	return ioctl( fd, mode, addr );
}
int I2C_write8( int fd, int addr, int d ) {
	uint8_t data[2];
	data[0] = (uint8_t)addr;
	data[1] = (uint8_t)d ;
	return write( fd, data, 2 );
}

int I2C_write5( int fd, int svNo, int onTime, int offTime ) {
/*	uint8_t data[5];

	data[0] = LED0_ON_L + 4 * svNo;
	data[1] = (uint8_t)(0x00ff & onTime);
	data[2] = (uint8_t)((0xff00 & onTime) >> 8);
	data[3] = (uint8_t)(0x00ff & offTime);
	data[4] = (uint8_t)((0xff00 & offTime) >> 8);
 	return write(fd, data, 5);
*/
	I2C_write8( fd, LED0_ON_L+4*svNo, onTime & 0xFF );
	I2C_write8( fd, LED0_ON_H+4*svNo, (onTime & 0xFF00) >> 8 );
	I2C_write8( fd, LED0_OFF_L+4*svNo, offTime & 0xFF );
	I2C_write8( fd, LED0_OFF_H+4*svNo, (offTime & 0xFF00) >> 8 );
	return 1;
}


int I2C_read8( int fd, int addr ) {
	uint8_t wdata;
	uint8_t rdata;
	wdata = (uint8_t)addr;
	if ( write( fd, &wdata, 1 ) != 1 ) {
		return -1;
	} 
	if ( read( fd, &rdata, 1 ) != 1 ) {
		return -1;
	}
	return (int)rdata;
}
void I2C_close( int fd ) {
	close( fd );
}


}

