using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FAAjinAIODevice : FAMemoryBaseDevice
    {
        private class AjinLibrary
        {

            public sealed class AxtMotDef
            {
                // IP COMMAND LIST
                public enum IPCOMMAND
                {
                    // PGM-1 Group Register
                    IPxyRANGERead = 0x00,        // PGM-1 RANGE READ, 16bit, 0xFFFF
                    IPxyRANGEWrite = 0x80,        // PGM-1 RANGE WRITE
                    IPxySTDRead = 0x01,        // PGM-1 START/STOP SPEED DATA READ, 16bit, 
                    IPxySTDWrite = 0x81,        // PGM-1 START/STOP SPEED DATA WRITE
                    IPxyOBJRead = 0x02,        // PGM-1 OBJECT SPEED DATA READ, 16bit, 
                    IPxyOBJWrite = 0x82,        // PGM-1 OBJECT SPEED DATA WRITE
                    IPxyRATE1Read = 0x03,        // PGM-1 RATE-1 DATA READ, 16bit, 0xFFFF
                    IPxyRATE1Write = 0x83,        // PGM-1 RATE-1 DATA WRITE
                    IPxyRATE2Read = 0x04,        // PGM-1 RATE-2 DATA READ, 16bit, 0xFFFF
                    IPxyRATE2Write = 0x84,        // PGM-1 RATE-2 DATA WRITE
                    IPxyRATE3Read = 0x05,        // PGM-1 RATE-3 DATA READ, 16bit, 0xFFFF
                    IPxyRATE3Write = 0x85,        // PGM-1 RATE-3 DATA WRITE
                    IPxyRCP12Read = 0x06,        // PGM-1 RATE CHANGE POINT 1-2 READ, 16bit, 0xFFFF
                    IPxyRCP12Write = 0x86,        // PGM-1 RATE CHANGE POINT 1-2 WRITE
                    IPxyRCP23Read = 0x07,        // PGM-1 RATE CHANGE POINT 2-3 READ, 16bit, 0xFFFF
                    IPxyRCP23Write = 0x87,        // PGM-1 RATE CHANGE POINT 2-3 WRITE
                    IPxySW1Read = 0x08,        // PGM-1 SW-1 DATA READ, 15bit, 0x7FFF
                    IPxySW1Write = 0x88,        // PGM-1 SW-1 DATA WRITE
                    IPxySW2Read = 0x09,        // PGM-1 SW-2 DATA READ, 15bit, 0x7FFF
                    IPxySW2Write = 0x89,        // PGM-1 SW-2 DATA WRITE
                    IPxyPWMRead = 0x0A,        // PGM-1 PWM 출력 설정 DATA READ(0~6), 3bit, 0x00
                    IPxyPWMWrite = 0x8A,        // PGM-1 PWM 출력 설정 DATA WRITE
                    IPxyREARRead = 0x0B,        // PGM-1 SLOW DOWN/REAR PULSE READ, 32bit, 0x00000000
                    IPxyREARWrite = 0x8B,        // PGM-1 SLOW DOWN/REAR PULSE WRITE
                    IPxySPDRead = 0x0C,        // PGM-1 현재 SPEED DATA READ, 16bit, 0x0000
                    IPxyNoOperation_8C = 0x8C,        // No operation
                    IPxySPDCMPRead = 0x0D,        // PGM-1 현재 SPEED 비교 DATA READ, 16bit, 0x0000
                    IPxySPDCMPWrite = 0x8D,        // PGM-1 현재 SPEED 비교 DATA WRITE
                    IPxyDRVPULSERead = 0x0E,        // PGM-1 DRIVE PULSE COUNTER READ, 32bit, 0x00000000
                    IPxyNoOperation_8E = 0x8E,        // No operation
                    IPxyPRESETPULSERead = 0x0F,        // PGM-1 PRESET PULSE DATA READ, 32bit, 0x00000000
                    IPxyNoOperation_8F = 0x8F,        // No operation
                    // PGM-1 Update Group Register
                    IPxyURANGERead = 0x10,        // PGM-1 UP-DATE RANGE READ, 16bit, 0xFFFF
                    IPxyURANGEWrite = 0x90,        // PGM-1 UP-DATE RANGE WRITE
                    IPxyUSTDRead = 0x11,        // PGM-1 UP-DATE START/STOP SPEED DATA READ, 16bit, 
                    IPxyUSTDWrite = 0x91,        // PGM-1 UP-DATE START/STOP SPEED DATA WRITE
                    IPxyUOBJRead = 0x12,        // PGM-1 UP-DATE OBJECT SPEED DATA READ, 16bit, 
                    IPxyUOBJWrite = 0x92,        // PGM-1 UP-DATE OBJECT SPEED DATA WRITE
                    IPxyURATE1Read = 0x13,        // PGM-1 UP-DATE RATE-1 DATA READ, 16bit, 0xFFFF
                    IPxyURATE1Write = 0x93,        // PGM-1 UP-DATE RATE-1 DATA WRITE
                    IPxyURATE2Read = 0x14,        // PGM-1 UP-DATE RATE-2 DATA READ, 16bit, 0xFFFF
                    IPxyURATE2Write = 0x94,        // PGM-1 UP-DATE RATE-2 DATA WRITE
                    IPxyURATE3Read = 0x15,        // PGM-1 UP-DATE RATE-3 DATA READ, 16bit, 0xFFFF
                    IPxyURATE3Write = 0x95,        // PGM-1 UP-DATE RATE-3 DATA WRITE
                    IPxyURCP12Read = 0x16,        // PGM-1 UP-DATE RATE CHANGE POINT 1-2 READ, 16bit, 0xFFFF
                    IPxyURCP12Write = 0x96,        // PGM-1 UP-DATE RATE CHANGE POINT 1-2 WRITE
                    IPxyURCP23Read = 0x17,        // PGM-1 UP-DATE RATE CHANGE POINT 2-3 READ, 16bit, 0xFFFF
                    IPxyURCP23Write = 0x97,        // PGM-1 UP-DATE RATE CHANGE POINT 2-3 WRITE
                    IPxyUSW1Read = 0x18,        // PGM-1 UP-DATE SW-1 DATA READ, 15bit, 0x7FFF
                    IPxyUSW1Write = 0x98,        // PGM-1 UP-DATE SW-1 DATA WRITE
                    IPxyUSW2Read = 0x19,        // PGM-1 UP-DATE SW-2 DATA READ, 15bit, 0x7FFF
                    IPxyUSW2Write = 0x99,        // PGM-1 UP-DATE SW-2 DATA WRITE
                    IPxyNoOperation_1A = 0x1A,        // No operation
                    IPxyNoOperation_9A = 0x9A,        // No operation
                    IPxyUREARRead = 0x1B,        // PGM-1 UP-DATE SLOW DOWN/REAR PULSE READ, 32bit, 0x00000000
                    IPxyUREARWrite = 0x9B,        // PGM-1 UP-DATE SLOW DOWN/REAR PULSE WRITE
                    IPxySPDRead_1C = 0x1C,        // PGM-1 UP-DATA CURRENT SPEED READ(Same with 0x0C)
                    IPxyNoOperation_9C = 0x9C,        // No operation
                    IPxySPDCMPRead_1D = 0x1D,        // PGM-1 현재 SPEED 비교 DATA READ(Same with 0x0D) 
                    IPxySPDCMPWrite_9D = 0x9D,        // PGM-1 현재 SPEED 비교 DATA WRITE(Same with 0x8D) 
                    IPxyACCPULSERead = 0x1E,        // PGM-1 가속 PULSE COUNTER READ, 32bit, 0x00000000
                    IPxyNoOperation_9E = 0x9E,        // No operation
                    IPxyPRESETPULSERead_1F = 0x1F,        // PGM-1 PRESET PULSE DATA READ(Same with 0x0F)
                    IPxyNoOperation_9F = 0x9F,        // No operation        
                    // PGM-2 Group Register
                    IPxyNoOperation_20 = 0x20,        // No operation
                    IPxyPPRESETDRV = 0xA0,        // +PRESET PULSE DRIVE, 32
                    IPxyNoOperation_21 = 0x21,        // No operation
                    IPxyPCONTDRV = 0xA1,        // +CONTINUOUS DRIVE
                    IPxyNoOperation_22 = 0x22,        // No operation
                    IPxyPSCH1DRV = 0xA2,        // +SIGNAL SEARCH-1 DRIVE
                    IPxyNoOperation_23 = 0x23,        // No operation
                    IPxyPSCH2DRV = 0xA3,        // +SIGNAL SEARCH-2 DRIVE
                    IPxyNoOperation_24 = 0x24,        // No operation
                    IPxyPORGDRV = 0xA4,        // +ORIGIN(원점) SEARCH DRIVE
                    IPxyNoOperation_25 = 0x25,        // No operation
                    IPxyMPRESETDRV = 0xA5,        // -PRESET PULSE DRIVE, 32
                    IPxyNoOperation_26 = 0x26,        // No operation
                    IPxyMCONTDRV = 0xA6,        // -CONTINUOUS DRIVE
                    IPxyNoOperation_27 = 0x27,        // No operation
                    IPxyMSCH1DRV = 0xA7,        // -SIGNAL SEARCH-1 DRIVE
                    IPxyNoOperation_28 = 0x28,        // No operation
                    IPxyMSCH2DRV = 0xA8,        // -SIGNAL SEARCH-2 DRIVE
                    IPxyNoOperation_29 = 0x29,        // No operation
                    IPxyMORGDRV = 0xA9,        // -ORIGIN(원점) SEARCH DRIVE
                    IPxyPULSEOVERRead = 0x2A,        // Preset/MPG drive override pulse data read
                    IPxyPULSEOVERWrite = 0xAA,        // PRESET PULSE DATA OVERRIDE(ON_BUSY)
                    IPxyNoOperation_2B = 0x2B,        // No operation
                    IPxySSTOPCMD = 0xAB,        // SLOW DOWN STOP
                    IPxyNoOperation_2C = 0x2C,        // No operation
                    IPxyESTOPCMD = 0xAC,        // EMERGENCY STOP
                    IPxyDRIVEMODERead = 0x2D,        // 드라이브 동작 설정 DATA READ
                    IPxyDRIVEMODEWrite = 0xAD,        // 드라이브 동작 설정 DATA WRITE
                    IPxyMPGCONRead = 0x2E,        // MPG OPERATION SETTING DATA READ, 3bit, 0x00    
                    IPxyMPGCONWrite = 0xAE,        // MPG OPERATION SETTING DATA WRITE                
                    IPxyPULSEMPGRead = 0x2F,        // MPG PRESET PULSE DATA READ, 32bit, 0x00000000
                    IPxyPULSEMPGWrite = 0xAF,        // MPG PRESET PULSE DATA WRITE                        
                    // Extension Group Register
                    IPxyNoOperation_30 = 0x30,        // No operation
                    IPxyPSPO1DRV = 0xB0,        // +SENSOR POSITIONING DRIVE I
                    IPxyNoOperation_31 = 0x31,        // No operation
                    IPxyMSPO1DRV = 0xB1,        // -SENSOR POSITIONING DRIVE I
                    IPxyNoOperation_32 = 0x32,        // No operation
                    IPxyPSPO2DRV = 0xB2,        // +SENSOR POSITIONING DRIVE II
                    IPxyNoOperation_33 = 0x33,        // No operation
                    IPxyMSPO2DRV = 0xB3,        // -SENSOR POSITIONING DRIVE II
                    IPxyNoOperation_34 = 0x34,        // No operation
                    IPxyPSPO3DRV = 0xB4,        // +SENSOR POSITIONING DRIVE III
                    IPxyNoOperation_35 = 0x35,        // No operation
                    IPxyMSPO3DRV = 0xB5,        // -SENSOR POSITIONING DRIVE III
                    IPxySWLMTCONRead = 0x36,        // SOFT LIMIT 설정 READ, 3bit, 0x00
                    IPxySWLMTCONWrite = 0xB6,        // SOFT LIMIT 설정 WRITE
                    IPxyMSWLMTCOMPRead = 0x37,        // -SOFT LIMIT 비교 레지스터 설정 READ, 32bit, 0x80000000
                    IPxyMSWLMTCOMPWrite = 0xB7,        // -SOFT LIMIT 비교 레지스터 설정 WRITE
                    IPxyPSWLMTCOMPRead = 0x38,        // +SOFT LIMIT 비교 레지스터 설정 READ, 32bit, 0x7FFFFFFF
                    IPxyPSWLMTCOMPWrite = 0xB8,        // +SOFT LIMIT 비교 레지스터 설정 WRITE
                    IPxyTRGCONRead = 0x39,        // TRIGGER MODE 설정 READ, 32bit, 0x00010000
                    IPxyTRGCONWrite = 0xB9,        // TRIGGER MODE 설정 WRITE
                    IPxyTRGCOMPRead = 0x3A,        // TRIGGER 비교 데이터 설정 READ, 32bit, 0x00000000
                    IPxyTRGCOMPWrite = 0xBA,        // TRIGGER 비교 데이터 설정 WRITE
                    IPxyICMRead = 0x3B,        // INTERNAL M-DATA 설정 READ, 32bit, 0x80000000
                    IPxyICMWrite = 0xBB,        // INTERNAL M-DATA 설정 WRITE
                    IPxyECMRead = 0x3C,        // EXTERNAL M-DATA 설정 READ, 32bit, 0x80000000
                    IPxyECMWrite = 0xBC,        // EXTERNAL M-DATA 설정 WRITE
                    IPxySTOPPWRead = 0x3D,        // Stop pulse width Read
                    IPxySTOPPWWrite = 0xBD,        // Stop pulse width Write
                    IPxyNoOperation_3E = 0x3E,        // No operation
                    IPxyNoOperation_BE = 0xBE,        // No operation
                    IPxyNoOperation_3F = 0x3F,        // No operation
                    IPxyTRGCMD = 0xBF,        // TRIG output signal generation command               
                    // Interpolation Group Registers
                    IPxCIRXCRead = 0x40,        // Circular interpolation X axis center point read
                    IPxCIRXCWrite = 0xC0,        // Circular interpolation X axis center point write 
                    IPxCIRYCRead = 0x41,        // Circular interpolation Y axis center point read 
                    IPxCIRYCWrite = 0xC1,        // Circular interpolation Y axis center point write  
                    IPxENDXRead = 0x42,        // Interpolation X axis end point read 
                    IPxENDXWrite = 0xC2,        // Interpolation X axis end point write  
                    IPxENDYRead = 0x43,        // Interpolation Y axis end point read  
                    IPxENDYWrite = 0xC3,        // Interpolation Y axis end point write  
                    IPxPTXENDRead = 0x44,        // Pattern interpolation X Queue data read
                    IPxPTXENDWrite = 0xC4,        // Pattern interpolation X Queue data with queue push 
                    IPxPTYENDRead = 0x45,        // Pattern interpolation Y Queue data read 
                    IPxPTYENDWrite = 0xC5,        // Pattern interpolation Y Queue data write
                    IPxPTQUEUERead = 0x46,        // Pattern interpolation Queue index read
                    IPxNoOperation_C6 = 0xC6,        // No operation
                    IPxNoOperation_47 = 0x47,        // No operation
                    IPxNoOperation_C7 = 0xC7,        // No operation
                    IPxNoOperation_48 = 0x48,        // No operation
                    IPxNoOperation_C8 = 0xC8,        // No operation
                    IPxNoOperation_49 = 0x49,        // No operation
                    IPxNoOperation_C9 = 0xC9,        // No operation
                    IPxINPSTATUSRead = 0x4A,        // Interpolation Status register read
                    IPxNoOperation_CA = 0xCA,        // No operation
                    IPxINPMODE_4B = 0x4B,        // Interpolation mode in Queue TOP contets
                    IPxLINPDRV = 0xCB,        // Linear interpolation with Queue push
                    IPxINPMODE_4C = 0x4C,        // Interpolation mode in Queue TOP contets
                    IPxCINPDRV = 0xCC,        // Circular interpolation with Queue push 
                    IPxBPINPMODE = 0x4D,        // Bit Pattern Interpolation mode in Queue TOP contets
                    IPxBPINPDRV = 0xCD,        // Bit pattern Drive
                    IPxNoOperation_4E = 0x4E,        // No Operation
                    IPxNoOperation_CE = 0xCE,        // No Operation 
                    IPxNoOperation_4F = 0x4F,        // No Operation 
                    IPxNoOperation_CF = 0xCF,        // No Operation         
                    // Arithemetic Group Register
                    IPxNoOperation_50 = 0x50,        // No Operation
                    IPxINPCLR = 0xD0,        // Initialize all interpolation control block
                    IPxINPMPOINTRead = 0x51,        // Interpolation deceleration manual point(unsigned) read
                    IPxINPMPOINTWrite = 0xD1,        // Interpolation deceleration manual point(unsigned) write
                    IPxNoOperation_52 = 0x52,        // No Operation
                    IPxINPCLRSWrite = 0xD2,        // Initialize interpolation control block with target selection
                    IPxNoOperation_53 = 0x53,        // No Operation
                    IPxINPDRVWrite = 0xD3,        // linear/circular drive start with queue data(Hold on mode), Restart on pause
                    IPxNoOperation_54 = 0x54,        // No operation
                    IPxNoOperation_D4 = 0xD4,        // No operation
                    IPxNoOperation_55 = 0x55,        // No operation
                    IPxARTSHOT = 0xD5,        // Arithmetic block One time execution
                    IPxARTSHOPERRead = 0x56,        // Arithmetic block shift and operation selection Read
                    IPxARTSHOPERWrite = 0xD6,        // Arithmetic block shift and operation selection Write
                    IPxARTSHRead = 0x57,        // Arithmetic block shift amount data Read
                    IPxARTSHWrite = 0xD7,        // Arithmetic block shift amount data Write
                    IPxARTSOURCERead = 0x58,        // Arithmetic block operand configure data Read
                    IPxARTSOURCEWrite = 0xD8,        // Arithmetic block operand configure data Write
                    IPxARTCRESULT1Read = 0x59,        // Arithmetic first compare result data Read
                    IPxNoOperation_D9 = 0xD9,        // No Operation
                    IPxARTCRESULT2Read = 0x5A,        // Arithmetic second compare result data Read
                    IPxNoOperation_DA = 0xDA,        // No Operation
                    IPxARTARESULT1Read = 0x5B,        // Arithmetic first algebraic result data Read
                    IPxNoOperation_DB = 0xDB,        // No Operation
                    IPxARTARESULT2Read = 0x5C,        // Arithmetic second algebraic result data Read
                    IPxNoOperation_DC = 0xDC,        // No operation
                    IPxARTUSERARead = 0x5D,        // Arithmetic block User operand A Read
                    IPxARTUSERAWrite = 0xDD,        // Arithmetic block User operand A Write
                    IPxARTUSERBRead = 0x5E,        // Arithmetic block User operand B Read
                    IPxARTUSERBWrite = 0xDE,        // Arithmetic block User operand B Write
                    IPxARTUSERCRead = 0x5F,        // Arithmetic block User operand C Read
                    IPxARTUSERCWrite = 0xDF,        // Arithmetic block User operand C Write        
                    // Scripter Group Register
                    IPySCRCON1Read = 0x40,        // 스크립트 동작 설정 레지스터-1 READ, 32bit, 0x00000000
                    IPySCRCON1Write = 0xC0,        // 스크립트 동작 설정 레지스터-1 WRITE
                    IPySCRCON2Read = 0x41,        // 스크립트 동작 설정 레지스터-2 READ, 32bit, 0x00000000
                    IPySCRCON2Write = 0xC1,        // 스크립트 동작 설정 레지스터-2 WRITE
                    IPySCRCON3Read = 0x42,        // 스크립트 동작 설정 레지스터-3 READ, 32bit, 0x00000000 
                    IPySCRCON3Write = 0xC2,        // 스크립트 동작 설정 레지스터-3 WRITE
                    IPySCRCONQRead = 0x43,        // 스크립트 동작 설정 레지스터-Queue READ, 32bit, 0x00000000
                    IPySCRCONQWrite = 0xC3,        // 스크립트 동작 설정 레지스터-Queue WRITE
                    IPySCRDATA1Read = 0x44,        // 스크립트 동작 데이터 레지스터-1 READ, 32bit, 0x00000000 
                    IPySCRDATA1Write = 0xC4,        // 스크립트 동작 데이터 레지스터-1 WRITE
                    IPySCRDATA2Read = 0x45,        // 스크립트 동작 데이터 레지스터-2 READ, 32bit, 0x00000000 
                    IPySCRDATA2Write = 0xC5,        // 스크립트 동작 데이터 레지스터-2 WRITE
                    IPySCRDATA3Read = 0x46,        // 스크립트 동작 데이터 레지스터-3 READ, 32bit, 0x00000000 
                    IPySCRDATA3Write = 0xC6,        // 스크립트 동작 데이터 레지스터-3 WRITE
                    IPySCRDATAQRead = 0x47,        // 스크립트 동작 데이터 레지스터-Queue READ, 32bit, 0x00000000 
                    IPySCRDATAQWrite = 0xC7,        // 스크립트 동작 데이터 레지스터-Queue WRITE
                    IPyNoOperation_48 = 0x48,        // No operation
                    IPySCRQCLR = 0xC8,        // 스크립트 Queue clear
                    IPySCRCQSIZERead = 0x49,        // 스크립트 동작 설정 Queue 인덱스 READ, 4bit, 0x00
                    IPyNoOperation_C9 = 0xC9,        // No operation
                    IPySCRDQSIZERead = 0x4A,        // 스크립트 동작 데이터 Queue 인덱스 READ, 4bit, 0x00
                    IPyNoOperation_CA = 0xCA,        // No operation
                    IPySCRQFLAGRead = 0x4B,        // 스크립트 Queue Full/Empty Flag READ, 4bit, 0x05
                    IPyNoOperation_CB = 0xCB,        // No operation
                    IPySCRQSIZECONRead = 0x4C,        // 스크립트 Queue size 설정(0~13) READ, 16bit, 0xD0D0
                    IPySCRQSIZECONWrite = 0xCC,        // 스크립트 Queue size 설정(0~13) WRITE
                    IPySCRQSTATUSRead = 0x4D,        // 스크립트 Queue status READ, 12bit, 0x005
                    IPyNoOperation_CD = 0xCD,        // No operation
                    IPyNoOperation_4E = 0x4E,        // No operation
                    IPyNoOperation_CE = 0xCE,        // No operation
                    IPyNoOperation_4F = 0x4F,        // No operation
                    IPyNoOperation_CF = 0xCF,        // No operation        
                    // Caption Group Register
                    IPyCAPCON1Read = 0x50,        // 갈무리 동작 설정 레지스터-1 READ, 32bit, 0x00000000
                    IPyCAPCON1Write = 0xD0,        // 갈무리 동작 설정 레지스터-1 WRITE
                    IPyCAPCON2Read = 0x51,        // 갈무리 동작 설정 레지스터-2 READ, 32bit, 0x00000000
                    IPyCAPCON2Write = 0xD1,        // 갈무리 동작 설정 레지스터-2 WRITE
                    IPyCAPCON3Read = 0x52,        // 갈무리 동작 설정 레지스터-3 READ, 32bit, 0x00000000 
                    IPyCAPCON3Write = 0xD2,        // 갈무리 동작 설정 레지스터-3 WRITE
                    IPyCAPCONQRead = 0x53,        // 갈무리 동작 설정 레지스터-Queue READ, 32bit, 0x00000000
                    IPyCAPCONQWrite = 0xD3,        // 갈무리 동작 설정 레지스터-Queue WRITE
                    IPyCAPDATA1Read = 0x54,        // 갈무리 동작 데이터 레지스터-1 READ, 32bit, 0x00000000 
                    IPyNoOperation_D4 = 0xD4,        // No operation
                    IPyCAPDATA2Read = 0x55,        // 갈무리 동작 데이터 레지스터-2 READ, 32bit, 0x00000000 
                    IPyNoOperation_D5 = 0xD5,        // No operation
                    IPyCAPDATA3Read = 0x56,        // 갈무리 동작 데이터 레지스터-3 READ, 32bit, 0x00000000 
                    IPyNoOperation_D6 = 0xD6,        // No operation
                    IPyCAPDATAQRead = 0x57,        // 갈무리 동작 데이터 레지스터-Queue READ, 32bit, 0x00000000 
                    IPyNoOperation_D7 = 0xD7,        // No operation
                    IPyNoOperation_58 = 0x58,        // No operation
                    IPyCAPQCLR = 0xD8,        // 갈무리 Queue clear
                    IPyCAPCQSIZERead = 0x59,        // 갈무리 동작 설정 Queue 인덱스 READ, 4bit, 0x00
                    IPyNoOperation_D9 = 0xD9,        // No operation
                    IPyCAPDQSIZERead = 0x5A,        // 갈무리 동작 데이터 Queue 인덱스 READ, 4bit, 0x00
                    IPyNoOperation_DA = 0xDA,        // No operation
                    IPyCAPQFLAGRead = 0x5B,        // 갈무리 Queue Full/Empty Flag READ, 4bit, 0x05
                    IPyNoOperation_DB = 0xDB,        // No operation
                    IPyCAPQSIZECONRead = 0x5C,        // 갈무리 Queue size 설정(0~13) READ, 16bit, 0xD0D0
                    IPyCAPQSIZECONWrite = 0xDC,        // 갈무리 Queue size 설정(0~13) WRITE
                    IPyCAPQSTATUSRead = 0x5D,        // 갈무리 Queue status READ, 12bit, 0x005
                    IPyNoOperation_DD = 0xDD,        // No operation
                    IPyNoOperation_5E = 0x5E,        // No operation
                    IPyNoOperation_DE = 0xDE,        // No operation
                    IPyNoOperation_5F = 0x5F,        // No operation
                    IPyNoOperation_DF = 0xDF,        // No operation         
                    // BUS - 1 Group Register
                    IPxyINCNTRead = 0x60,        // INTERNAL COUNTER DATA READ(Signed), 32bit, 0x00000000
                    IPxyINCNTWrite = 0xE0,        // INTERNAL COUNTER DATA WRITE(Signed)
                    IPxyINCNTCMPRead = 0x61,        // INTERNAL COUNTER COMPARATE DATA READ(Signed), 32bit, 0x00000000
                    IPxyINCNTCMPWrite = 0xE1,        // INTERNAL COUNTER COMPARATE DATA WRITE(Signed)
                    IPxyINCNTSCALERead = 0x62,        // INTERNAL COUNTER PRE-SCALE DATA READ, 8bit, 0x00
                    IPxyINCNTSCALEWrite = 0xE2,        // INTERNAL COUNTER PRE-SCALE DATA WRITE
                    IPxyICPRead = 0x63,        // INTERNAL COUNTER P-DATA READ, 32bit, 0x7FFFFFFF
                    IPxyICPWrite = 0xE3,        // INTERNAL COUNTER P-DATA WRITE
                    IPxyEXCNTRead = 0x64,        // EXTERNAL COUNTER DATA READ READ(Signed), 32bit, 0x00000000
                    IPxyEXCNTWrite = 0xE4,        // EXTERNAL COUNTER DATA READ WRITE(Signed)
                    IPxyEXCNTCMPRead = 0x65,        // EXTERNAL COUNTER COMPARATE DATA READ(Signed), 32bit, 0x00000000
                    IPxyEXCNTCMPWrite = 0xE5,        // EXTERNAL COUNTER COMPARATE DATA WRITE(Signed)
                    IPxyEXCNTSCALERead = 0x66,        // EXTERNAL COUNTER PRE-SCALE DATA READ, 8bit, 0x00
                    IPxyEXCNTSCALEWrite = 0xE6,        // EXTERNAL COUNTER PRE-SCALE DATA WRITE
                    IPxyEXPRead = 0x67,        // EXTERNAL COUNTER P-DATA READ, 32bit, 0x7FFFFFFF
                    IPxyEXPWrite = 0xE7,        // EXTERNAL COUNTER P-DATA WRITE
                    IPxyEXSPDRead = 0x68,        // EXTERNAL SPEED DATA READ, 32bit, 0x00000000
                    IPxyNoOperation_E8 = 0xE8,        // No operation
                    IPxyEXSPDCMPRead = 0x69,        // EXTERNAL SPEED COMPARATE DATA READ, 32bit, 0x00000000
                    IPxyEXSPDCMPWrite = 0xE9,        // EXTERNAL SPEED COMPARATE DATA WRITE
                    IPxyEXFILTERDRead = 0x6A,        // 외부 센서 필터 대역폭 설정 DATA READ, 32bit, 0x00050005
                    IPxyEXFILTERDWrite = 0xEA,        // 외부 센서 필터 대역폭 설정 DATA WRITE
                    IPxyOFFREGIONRead = 0x6B,        // OFF-RANGE DATA READ, 8bit, 0x00
                    IPxyOFFREGIONWrite = 0xEB,        // OFF-RANGE DATA WRITE
                    IPxyDEVIATIONRead = 0x6C,        // DEVIATION DATA READ, 16bit, 0x0000
                    IPxyNoOperation_EC = 0xEC,        // No operation
                    IPxyPGMCHRead = 0x6D,        // PGM REGISTER CHANGE DATA READ
                    IPxyPGMCHWrite = 0xED,        // PGM REGISTER CHANGE DATA WRITE
                    IPxyCOMPCONRead = 0x6E,        // COMPARE REGISTER INPUT CHANGE DATA READ
                    IPxyCOMPCONWrite = 0xEE,        // COMPARE REGISTER INPUT CHANGE DATA WRITE
                    IPxyNoOperation_6F = 0x6F,        // No operation
                    IPxyNoOperation_EF = 0xEF,        // No operation          
                    // BUS - 2 Group Register
                    IPxyFUNCONRead = 0x70,        // 칩 기능 설정 DATA READ,
                    IPxyFUNCONWrite = 0xF0,        // 칩 기능 설정 DATA WRITE
                    IPxyMODE1Read = 0x71,        // MODE1 DATA READ,
                    IPxyMODE1Write = 0xF1,        // MODE1 DATA WRITE
                    IPxyMODE2Read = 0x72,        // MODE2 DATA READ,
                    IPxyMODE2Write = 0xF2,        // MODE2 DATA WRITE
                    IPxyUIODATARead = 0x73,        // UNIVERSAL IN READ,
                    IPxyUIODATAWrite = 0xF3,        // UNIVERSAL OUT WRITE
                    IPxyENDSTATUSRead = 0x74,        // END STATUS DATA READ,
                    IPxyCLIMCLR = 0xF4,        // Complete limit stop clear command
                    IPxyMECHRead = 0x75,        // MECHANICAL SIGNAL DATA READ, 13bit
                    IPxyNoOperation_F5 = 0xF5,        // No operation
                    IPxyDRVSTATUSRead = 0x76,        // DRIVE STATE DATA READ, 20bit
                    IPxyNoOperation_F6 = 0xF6,        // No operation
                    IPxyEXCNTCLRRead = 0x77,        // EXTERNAL COUNTER 설정 DATA READ, 9bit, 0x00
                    IPxyEXCNTCLRWrite = 0xF7,        // EXTERNAL COUNTER 설정 DATA WRITE
                    IPxyNoOperation_78 = 0x78,        // No operation
                    IPxySWRESET = 0xF8,        // REGISTER CLEAR(INITIALIZATION), Software reset
                    IPxyINTFLAG1Read = 0x79,        // Interrupt Flag1 READ, 32bit, 0x00000000
                    IPxyINTFLAG1CLRWrite = 0xF9,        // Interrupt Flag1 Clear data write command.
                    IPxyINTMASK1Read = 0x7A,        // Interrupt Mask1 READ, 32bit, 0x00000001
                    IPxyINTMASK1Write = 0xFA,        // Interrupt Mask1 WRITE
                    IPxyUIOMODERead = 0x7B,        // UIO MODE DATA READ, 12bit, 0x01F
                    IPxyUIOMODEWrite = 0xFB,        // UIO MODE DATA WRITE
                    IPxyINTFLAG2Read = 0x7C,        // Interrupt Flag2 READ, 32bit, 0x00000000
                    IPxyINTFLAG2CLRWrite = 0xFC,        // Interrupt Flag2 Clear data write command.
                    IPxyINTMASK2Read = 0x7D,        // Interrupt Mask2 READ, 32bit, 0x00000001
                    IPxyINTMASK2Write = 0xFD,        // Interrupt Mask2 WRITE
                    IPxyINTUSERCONRead = 0x7E,        // User interrupt selection control.
                    IPxyINTUSERCONWrite = 0xFE,        // User interrupt selection control. 
                    IPxyNoOperation_7F = 0x7F,        // No operation
                    IPxyINTGENCMD = 0xFF         // Interrupt generation command.
                };

                // CAMC-QI COMMAND LIST
                public enum QICOMMAND
                {
                    // Previous register and etc Registers
                    QiPRANGERead = 0x00,        // Previous RANGE READ
                    QiPRANGEWrite = 0x80,        // Previous RANGE WRITE
                    QiPSTDRead = 0x01,        // Previous START/STOP SPEED DATA READ
                    QiPSTDWrite = 0x81,        // Previous START/STOP SPEED DATA WRITE
                    QiPOBJRead = 0x02,        // Previous OBJECT SPEED DATA READ
                    QiPOBJWrite = 0x82,        // Previous OBJECT SPEED DATA WRITE
                    QiPRATE1Read = 0x03,        // Previous RATE-1 DATA READ
                    QiPRATE1Write = 0x83,        // Previous RATE-1 DATA WRITE
                    QiPRATE2Read = 0x04,        // Previous RATE-2 DATA READ
                    QiPRATE2Write = 0x84,        // Previous RATE-2 DATA WRITE
                    QiPSW1Read = 0x05,        // Previous SW-1 DATA READ
                    QiPSW1Write = 0x85,        // Previous SW-1 DATA WRITE
                    QiPSW2Read = 0x06,        // Previous SW-2 DATA READ
                    QiPSW2Write = 0x86,        // Previous SW-2 DATA WRITE
                    QiPDCFGRead = 0x07,        // Previous Drive configure data READ
                    QiPDCFGWrite = 0x87,        // Previous Drive configure data WRITE
                    QiPREARRead = 0x08,        // Previous SLOW DOWN/REAR PULSE READ
                    QiPREARWrite = 0x88,        // Previous SLOW DOWN/REAR PULSE WRITE
                    QiPPOSRead = 0x09,        // Previous Drive pulse amount data/Interpolation end position READ
                    QiPPOSWrite = 0x89,        // Previous Drive pulse amount data/Interpolation end position WRITE 
                    QiPCENTRead = 0x0A,        // Previous Circular Int. center/Master axis target position for multiple chip linear int. READ
                    QiPCENTWrite = 0x8A,        // Previous Circular Int. center/Master axis target position for multiple chip linear int. WRITE
                    QiPISNUMRead = 0x0B,        // Previous Interpolation step number READ
                    QiPISNUMWrite = 0x8B,        // Previous Interpolation step number WRITE
                    QiNoOperation_0C = 0x0C,        // No operation
                    QiCLRPRE = 0x8C,        // Clear previous driving data Queue.
                    QiNoOperation_0D = 0x0D,        // No operation
                    QiPOPPRE = 0x8D,        // Pop and shift data of previous driving data Queue.
                    QiPPORTMARestore = 0x0E,        // Restore data ports.
                    QiPPORTMABackup = 0x8E,        // Backup data ports.
                    QiCURSPDRead = 0x0F,        // Current SPEED DATA READ
                    QiNoOperation_8F = 0x8F,        // No operation            
                    // Working Registers
                    QiRANGERead = 0x10,        // RANGE READ
                    QiRANGEWrite = 0x90,        // RANGE WRITE
                    QiSTDRead = 0x11,        // START/STOP SPEED DATA READ
                    QiSTDWrite = 0x91,        // START/STOP SPEED DATA WRITE
                    QiOBJRead = 0x12,        // OBJECT SPEED DATA READ
                    QiOBJWrite = 0x92,        // OBJECT SPEED DATA WRITE
                    QiRATE1Read = 0x13,        // RATE-1 DATA READ
                    QiRATE1Write = 0x93,        // RATE-1 DATA WRITE
                    QiRATE2Read = 0x14,        // RATE-2 DATA READ
                    QiRATE2Write = 0x94,        // RATE-2 DATA WRITE
                    QiSW1Read = 0x15,        // SW-1 DATA READ
                    QiSW1Write = 0x95,        // SW-1 DATA WRITE
                    QiSW2Read = 0x16,        // SW-2 DATA READ
                    QiSW2Write = 0x96,        // SW-2 DATA WRITE
                    QiDCFGRead = 0x17,        // Drive configure data READ
                    QiDCFGWrite = 0x97,        // Drive configure data WRITE
                    QiREARRead = 0x18,        // SLOW DOWN/REAR PULSE READ
                    QiREARWrite = 0x98,        // SLOW DOWN/REAR PULSE WRITE
                    QiPOSRead = 0x19,        // Drive pulse amount data/Interpolation end position READ
                    QiPOSWrite = 0x99,        // Drive pulse amount data/Interpolation end position WRITE 
                    QiCENTRead = 0x1A,        // Circular Int. center/Master axis target position for multiple chip linear int. READ
                    QiCENTWrite = 0x9A,        // Circular Int. center/Master axis target position for multiple chip linear int. WRITE
                    QiISNUMRead = 0x1B,        // Interpolation step number READ
                    QiISNUMWrite = 0x9B,        // Interpolation step number WRITE
                    QiREMAIN = 0x1C,        // Remain pulse data after stopping preset drive function abnormally.
                    QiNoOperation_9C = 0x9C,        // No operation
                    QiOBJORGRead = 0x1F,        // Original search object speed READ
                    QiOBJORGWrite = 0x9F,        // Original search object speed WRITE            
                    // Universal In/Out setting
                    QiUIOMRead = 0x1D,        // Universal in/out terminal mode READ
                    QiUIOMWrite = 0x9D,        // Universal in/out terminal mode WRITE
                    QiUIORead = 0x1E,        // Universal in/out terminal mode READ
                    QiUIOWrite = 0x9E,        // Universal in/out terminal mode WRIT            
                    // Drive start command
                    QiNoOperation_20 = 0x20,        // No operation.
                    QiSTRN = 0xA0,        // Normal profile mode drive start.(STD => OBJ => STD)
                    QiNoOperation_21 = 0x21,        // No operation.
                    QiSTRO = 0xA0,        // Start at OBJ profile mode drive start.(OBJ => STD)
                    QiNoOperation_22 = 0x22,        // No operation.
                    QiSTRCO = 0xA0,        // Constant speed profile #1 drive start.(OBJ)
                    QiNoOperation_23 = 0x23,        // No operation.
                    QiSTRCS = 0xA0,        // Constant speed profile #2 drive start.(STD)
                    QiNoOperation_60 = 0x5C,        // No operation.
                    QiASTRN = 0xDC,        // Normal profile mode drive start with DCFG7~0 bit data in DATAPL0 port.(STD => OBJ => STD)
                    QiNoOperation_61 = 0x5D,        // No operation.
                    QiASTRO = 0xDD,        // Start at OBJ profile mode drive start with DCFG7~0 bit data in DATAPL0 port.(OBJ => STD)
                    QiNoOperation_62 = 0x5E,        // No operation.
                    QiASTRCO = 0xDE,        // Constant speed profile #1 drive start with DCFG7~0 bit data in DATAPL0 port.(OBJ)
                    QiNoOperation_63 = 0x5F,        // No operation.
                    QiASTRCS = 0xDF,        // Constant speed profile #2 drive start with DCFG7~0 bit data in DATAPL0 port.(STD)            
                    // Drive control command
                    QiNoOperation_24 = 0x24,        // No operation.
                    QiSSTOP = 0xA4,        // Slow Down stop.
                    QiNoOperation_25 = 0x25,        // No operation.
                    QiSTOP = 0xA5,        // Immediately stop.
                    QiNoOperation_26 = 0x26,        // No operation.
                    QiSQRO1 = 0xA6,        // Output one shot of the start pulse form SQSTR1 terminal.
                    QiNoOperation_27 = 0x27,        // No operation.
                    QiSQRO2 = 0xA7,        // Output one shot of the start pulse form SQSTR2 terminal.
                    QiNoOperation_28 = 0x28,        // No operation.
                    QiSQRI1 = 0xA8,        // Execution sync start function same as SQSTR1 input.
                    QiNoOperation_29 = 0x29,        // No operation.
                    QiSQRI2 = 0xA9,        // Execution sync start function same as SQSTR2 input.
                    QiNoOperation_2A = 0x2A,        // No operation
                    QiSQSTP1 = 0xAA,        // Output one shot of the stop pulse from SQSTP1 terminal.
                    QiNoOperation_2B = 0x2B,        // No operation.
                    QiSQSTP2 = 0xAB,        // Output one shot of the stop pulse from SQSTP2 terminal.
                    QiISCNTRead = 0x2C,        // Interpolation stop counter value READ.
                    QiNoOperation_AC = 0xAC,        // No operation.
                    QiISACNTRead = 0x2D,        // Interpolation step counter READ for advanced deceleration mode . 
                    QiNoOperation_AD = 0xAD,        // No operation.
                    QiNoOperation_2E = 0x2E,        // No operation.
                    QiESTOP = 0xAE,        // Emergency stop all axis.
                    QiNoOperation_2F = 0x2F,        // No operation
                    QiSWRESET = 0xAF,		// Software reset(all axis).            
                    // QiNoOperation_30      = 0x30,        // Driven pulse amount during last driving(Interpolation step counter for path move).
                    // QiDRPCNTRead          = 0xB0,        // No operation
                    QiDRPCNTRead = 0x30,        // No operation
                    QiNoOperation_B0 = 0xB0,        // Driven pulse amount during last driving(Interpolation step counter for path move).
                    QiNoOperation_31 = 0x31,        // No operation
                    QiINTGEN = 0xB1,        // Interrupt generation command.
                    // Peripheral function setting.
                    QiNoOperation_33 = 0x32,        // No operation.
                    QiTRGQPOP = 0xB2,        // Pop and shift data in trigger position queue.
                    QiTRTMCFRead = 0x33,        // Trigger/Timer configure READ.
                    QiTRTMCFWrite = 0xB3,        // Trigger/Timer configure WRITE.
                    QiSNSMTRead = 0x34,        // Software negative limit position READ.
                    QiSNSMTWrite = 0xB4,        // Software negative limit position WRITE.
                    QiSPSMTRead = 0x35,        // Software positive limit position READ.
                    QiSPSMTWrite = 0xB5,        // Software positive limit position WRITE.
                    QiTRGPWRead = 0x36,        // Trigger pulse width READ.
                    QiTRGPWWrite = 0xB6,        // Trigger pulse width WRITE.
                    QiTRGSPRead = 0x37,        // Trigger function start position READ.
                    QiTRGSPWrite = 0xB7,        // Trigger function start position WRITE.
                    QiTRGEPRead = 0x38,        // Trigger function end position READ.
                    QiTRGEPWrite = 0xB8,        // Trigger function end position WRITE.
                    QiPTRGPOSRead = 0x39,        // Trigger position or period queue data READ.
                    QiPTRGPOSWrite = 0xB9,        // Push trigger position or period queue.
                    QiNoOperation_3A = 0x3A,        // No operation.
                    QiCLRTRIG = 0xBA,        // Clear trigger position or period queue.
                    QiNoOperation_3B = 0x3B,        // No operation.
                    QiTRGGEN = 0xBB,        // Generate one shot trigger pulse.
                    QiTMRP1Read = 0x3C,        // Timer #1 period data READ.
                    QiTMRP1Write = 0xBC,        // Timer #1 period data WRITE.
                    QiTMRP2Read = 0x3D,        // Timer #2 period data READ.
                    QiTMRP2Write = 0xBD,        // Timer #2 period data WRITE.
                    QiTMR1GENstop = 0x3E,        // Timer #1 stop.
                    QiTMR1GENstart = 0xBE,        // Timer #1 start.
                    QiTMR2GENstop = 0x3F,        // Timer #2 stop.
                    QiTMR2GENstart = 0xBF,        // Timer #2 start.
                    QiERCReset = 0x60,        // ERC signal reset.
                    QiERCSet = 0xE0,        // ERC signal set.            
                    //Script1/2/3 setting registers         
                    QiSCRCON1Read = 0x40,        // Script1 control queue register READ.
                    QiSCRCON1Write = 0xC0,        // Script1 control queue register WRITE.
                    QiSCRCMD1Read = 0x41,        // Script1 command queue register READ.
                    QiSCRCMD1Write = 0xC1,        // Script1 command queue register WRITE.
                    QiSCRDAT1Read = 0x42,        // Script1 execution data queue register READ.
                    QiSCRDAT1Write = 0xC2,        // Script1 execution data queue register WRITE.
                    QiCQ1Read = 0x43,        // Script1 captured data queue register(top of depth 15 Queue)READ.
                    QiNoOperation_C3 = 0xC3,        // No operation.
                    QiSCRCFG1Read = 0x44,        // Script1 flag control register READ.
                    QiSCRCFG1Write = 0xC4,        // Script1 flag control register WRITE. 
                    QiSCRCON2Read = 0x45,        // Script2 control queue register READ.
                    QiSCRCON2Write = 0xC5,        // Script2 control queue register WRITE.
                    QiSCRCMD2Read = 0x46,        // Script2 command queue register READ.
                    QiSCRCMD2Write = 0xC6,        // Script2 command queue register WRITE.
                    QiSCRDAT2Read = 0x47,        // Script2 execution data queue register READ.
                    QiSCRDAT2Write = 0xC7,        // Script2 execution data queue register WRITE.
                    QiCQ2Read = 0x48,        // Script2 captured data queue register(top of depth 15 Queue)READ.
                    QiNoOperation_C8 = 0xC8,        // No operation.
                    QiSCRCFG2Read = 0x49,        // Script2 flag control register READ.
                    QiSCRCFG2Write = 0xC9,        // Script2 flag control register WRITE. 
                    QiSCRCON3Read = 0x4A,        // Script3 control register READ.
                    QiSCRCON3Write = 0xCA,        // Script3 control register WRITE.
                    QiSCRCMD3Read = 0x4B,        // Script3 command register READ.
                    QiSCRCMD3Write = 0xCB,        // Script3 command register WRITE.
                    QiSCRDAT3Read = 0x4C,        // Script3 execution data register READ.
                    QiSCRDAT3Write = 0xCC,        // Script3 execution data register WRITE.
                    QiCQ3Read = 0x4D,        // Script3 captured data register READ.
                    QiNoOperation_CD = 0xCD,        // No operation.
                    QiNoOperation_4E = 0x4E,        // No operation.
                    QiNoOperation_CE = 0xCE,        // No operation.
                    QiNoOperation_4F = 0x4F,        // No operation.
                    QiNoOperation_CF = 0xCF,        // [No operation code for script reservation command].            
                    //Script4 and Script status setting registers 
                    QiSCRCON4Read = 0x50,        // Script4 control register READ.
                    QiSCRCON4Write = 0xD0,        // Script4 control register WRITE.
                    QiSCRCMD4Read = 0x51,        // Script4 command register READ.
                    QiSCRCMD4Write = 0xD1,        // Script4 command register WRITE.
                    QiSCRDAT4Read = 0x52,        // Script4 execution data register READ.
                    QiSCRDAT4Write = 0xD2,        // Script4 execution data register WRITE.
                    QiCQ4Read = 0x53,        // Script4 captured data register READ.
                    QiNoOperation_D3 = 0xD3,        // No operation.
                    QiSCRTGRead = 0x54,        // Target source data setting READ.
                    QiSCRTGWrite = 0xD4,        // Target source data setting WRITE.
                    QiSCRSTAT1Read = 0x55,        // Script status #1 READ.
                    QiNoOperation_D5 = 0xD5,        // No operation.
                    QiSCRSTAT2Read = 0x56,        // Script status #2 READ.
                    QiNoOperation_D6 = 0xD6,        // No operation.
                    QiNoOperation_57 = 0x57,        // No operation.
                    QiINITSQWrite = 0xD7,        // Initialize script queues with target selection.
                    QiNoOperation_58 = 0x58,        // No operation.
                    QiINITCQWrite = 0xD8,        // Initialize captured data queue with target selection.
                    QiSCRMRead = 0x59,        // Set enable mode with target selection READ.
                    QiSCRMWrite = 0xD9,        // Set enable mode with target selection WRITE.
                    QiNoOperation_5A = 0x5A,        // No operation.
                    QiSQ1POP = 0xDA,        // Pop and shift data of script1 queue.
                    QiNoOperation_5B = 0x5B,        // No operation.
                    QiSQ2POP = 0xDB,        // Pop and shift data of script2 queue.            
                    //Counter function registers            
                    QiCNTLBRead = 0x61,        // Counter lower bound data READ.
                    QiCNTLBWrite = 0xE1,        // Counter lower bound data WRITE.
                    QiCNTUBRead = 0x62,        // Counter upper bound data READ.
                    QiCNTUBWrite = 0xE2,        // Counter upper bound data WRITE.
                    QiCNTCF1Read = 0x63,        // Counter configure #1 READ.
                    QiCNTCF1Write = 0xE3,        // Counter configure #1 WRITE.
                    QiCNTCF2Read = 0x64,        // Counter configure #2 READ.
                    QiCNTCF2Write = 0xE4,        // Counter configure #2 WRITE.
                    QiCNTCF3Read = 0x65,        // Counter configure #3 READ.
                    QiCNTCF3Write = 0xE5,        // Counter configure #3 WRITE.
                    QiCNT1Read = 0x66,        // Counter #1 data READ.
                    QiCNT1Write = 0xE6,        // Counter #1 data WRITE.
                    QiCNT2Read = 0x67,        // Counter #2 data READ.
                    QiCNT2Write = 0xE7,        // Counter #2 data WRITE.
                    QiCNT3Read = 0x68,        // Counter #3 data READ.
                    QiCNT3Write = 0xE8,        // Counter #3 data WRITE.
                    QiCNT4Read = 0x69,        // Counter #4 data READ.
                    QiCNT4Write = 0xE9,        // Counter #4 data WRITE.
                    QiCNT5Read = 0x6A,        // Counter #5 data READ.
                    QiCNT5Write = 0xEA,        // Counter #5 data WRITE.
                    QiCNTC1Read = 0x6B,        // Counter #1 comparator's data READ.
                    QiCNTC1Write = 0xEB,        // Counter #1 comparator's data WRITE.
                    QiCNTC2Read = 0x6C,        // Counter #2 comparator's data READ.
                    QiCNTC2Write = 0xEC,        // Counter #2 comparator's data WRITE.
                    QiCNTC3Read = 0x6D,        // Counter #3 comparator's data READ.
                    QiCNTC3Write = 0xED,        // Counter #3 comparator's data WRITE.
                    QiCNTC4Read = 0x6E,        // Counter #4 comparator's data READ.
                    QiCNTC4Write = 0xEE,        // Counter #4 comparator's data WRITE.
                    QiCNTC5Read = 0x6F,        // Counter #5 comparator's data READ.
                    QiCNTC5Write = 0xEF,        // Counter #5 comparator's data WRITE.            
                    //Configure and Status registers        
                    QiUCFG1Read = 0x70,        // Configure register #1 READ.
                    QiUCFG1Write = 0xF0,        // Configure register #1 WRITE.
                    QiUCFG2Read = 0x71,        // Configure register #2 READ.
                    QiUCFG2Write = 0xF1,        // Configure register #2 WRITE.
                    QiUCFG3Read = 0x72,        // Configure register #3 READ.
                    QiUCFG3Write = 0xF2,        // Configure register #3 WRITE.
                    QiUCFG4Read = 0x73,        // Configure register #4 READ.
                    QiUCFG4Write = 0xF3,        // Configure register #4 WRITE.
                    QiNoOperation_74 = 0x74,        // No operation.
                    QiNoOperation_F4 = 0xF4,        // No operation.
                    QiNoOperation_75 = 0x75,        // No operation.
                    QiNoOperation_F5 = 0xF5,        // No operation.
                    QiNoOperation_76 = 0x76,        // No operation.
                    QiNoOperation_F6 = 0xF6,        // No operation.
                    QiIMASK1Read = 0x77,        // Interrupt bank#1 mask register READ.
                    QiIMASK1Write = 0xF7,        // Interrupt bank#1 mask register WRITE.
                    QiIMASK2Read = 0x78,        // Interrupt bank#2 mask register READ.
                    QiIMASK2Write = 0xF8,        // Interrupt bank#2 mask register WRITE.
                    QiSTAT1Read = 0x79,        // Status register #1(END STATUS)READ.
                    QiESCLR = 0xF9,        // Status register #1(END STATUS) Clear.
                    QiSTAT2Read = 0x7A,        // Status register #2 READ.
                    QiNoOperation_FA = 0xFA,        // No operation.
                    QiSTAT3Read = 0x7B,        // Status register #3 READ.
                    QiNoOperation_FB = 0xFB,        // No operation.
                    QiSTAT4Read = 0x7C,        // Status register #4 READ.
                    QiNoOperation_FC = 0xFC,        // No operation.
                    QiSTAT5Read = 0x7D,        // Status register #5 READ.
                    QiNoOperation_FD = 0xFD,        // No operation.
                    QiIFLAG1Read = 0x7E,        // Interrupt bank #1 flag READ.
                    QiIFLAG1Clear = 0xFE,        // Interrupt bank #1 flag Clear.
                    QiIFLAG2Read = 0x7F,        // Interrupt bank #2 flag READ.
                    QiIFLAG2Clear = 0xFF         // Interrupt bank #2 flag Clear.
                };

                // EVENT LIST
                public enum IPEVENT
                {
                    EVENT_IPNONE = 0x00,
                    EVENT_IPDRIVE_END = 0x01,
                    EVENT_IPPRESETDRIVE_START = 0x02,
                    EVENT_IPPRESETDRIVE_END = 0x03,
                    EVENT_IPCONTINOUSDRIVE_START = 0x04,
                    EVENT_IPCONTINOUSDRIVE_END = 0x05,
                    EVENT_IPSIGNAL_SEARCH_1_START = 0x06,
                    EVENT_IPSIGNAL_SEARCH_1_END = 0x07,
                    EVENT_IPSIGNAL_SEARCH_2_START = 0x08,
                    EVENT_IPSIGNAL_SEARCH_2_END = 0x09,
                    EVENT_IPORIGIN_DETECT_START = 0x0A,
                    EVENT_IPORIGIN_DETECT_END = 0x0B,
                    EVENT_IPSPEED_UP = 0x0C,
                    EVENT_IPSPEED_CONST = 0x0D,
                    EVENT_IPSPEED_DOWN = 0x0E,
                    EVENT_IPICL = 0x0F,
                    EVENT_IPICE = 0x10,
                    EVENT_IPICG = 0x11,
                    EVENT_IPECL = 0x12,
                    EVENT_IPECE = 0x13,
                    EVENT_IPECG = 0x14,
                    EVENT_IPEPCE = 0x15,
                    EVENT_IPEPCL = 0x16,
                    EVENT_IPEPCG = 0x17,
                    EVENT_IPSPL = 0x18,
                    EVENT_IPSPE = 0x19,
                    EVENT_IPSPG = 0x1A,
                    EVENT_IPSP12L = 0x1B,
                    EVENT_IPSP12E = 0x1C,
                    EVENT_IPSP12G = 0x1D,
                    EVENT_IPSP23L = 0x1E,
                    EVENT_IPSP23E = 0x1F,
                    EVENT_IPSP23G = 0x20,
                    EVENT_IPOBJECT_SPEED = 0x21,
                    EVENT_IPSS_SPEED = 0x22,
                    EVENT_IPESTOP = 0x23,
                    EVENT_IPSSTOP = 0x24,
                    EVENT_IPPELM = 0x25,
                    EVENT_IPNELM = 0x26,
                    EVENT_IPPSLM = 0x27,
                    EVENT_IPNSLM = 0x28,
                    EVENT_IPDEVIATION_ERROR = 0x29,
                    EVENT_IPDATA_ERROR = 0x2A,
                    EVENT_IPALARM_ERROR = 0x2B,
                    EVENT_IPESTOP_COMMAND = 0x2C,
                    EVENT_IPSSTOP_COMMAND = 0x2D,
                    EVENT_IPESTOP_SIGNAL = 0x2E,
                    EVENT_IPSSTOP_SIGNAL = 0x2F,
                    EVENT_IPELM = 0x30,
                    EVENT_IPSLM = 0x31,
                    EVENT_IPINPOSITION = 0x32,
                    EVENT_IPINOUT0_HIGH = 0x33,
                    EVENT_IPINOUT0_LOW = 0x34,
                    EVENT_IPINOUT1_HIGH = 0x35,
                    EVENT_IPINOUT1_LOW = 0x36,
                    EVENT_IPINOUT2_HIGH = 0x37,
                    EVENT_IPINOUT2_LOW = 0x38,
                    EVENT_IPINOUT3_HIGH = 0x39,
                    EVENT_IPINOUT3_LOW = 0x3A,
                    EVENT_IPINOUT4_HIGH = 0x3B,
                    EVENT_IPINOUT4_LOW = 0x3C,
                    EVENT_IPINOUT5_HIGH = 0x3D,
                    EVENT_IPINOUT5_LOW = 0x3E,
                    EVENT_IPINOUT6_HIGH = 0x3F,
                    EVENT_IPINOUT6_LOW = 0x40,
                    EVENT_IPINOUT7_HIGH = 0x41,
                    EVENT_IPINOUT7_LOW = 0x42,
                    EVENT_IPINOUT8_HIGH = 0x43,
                    EVENT_IPINOUT8_LOW = 0x44,
                    EVENT_IPINOUT9_HIGH = 0x45,
                    EVENT_IPINOUT9_LOW = 0x46,
                    EVENT_IPINOUT10_HIGH = 0x47,
                    EVENT_IPINOUT10_LOW = 0x48,
                    EVENT_IPINOUT11_HIGH = 0x49,
                    EVENT_IPINOUT11_LOW = 0x4A,
                    EVENT_IPSENSOR_DRIVE1_START = 0x4B,
                    EVENT_IPSENSOR_DRIVE1_END = 0x4C,
                    EVENT_IPSENSOR_DRIVE2_START = 0x4D,
                    EVENT_IPSENSOR_DRIVE2_END = 0x4E,
                    EVENT_IPSENSOR_DRIVE3_START = 0x4F,
                    EVENT_IPSENSOR_DRIVE3_END = 0x50,
                    EVENT_IP1STCOUNTER_NDATA_CLEAR = 0x51,
                    EVENT_IP2NDCOUNTER_NDATA_CLEAR = 0x52,
                    EVENT_IPMARK_SIGNAL_HIGH = 0x53,
                    EVENT_IPMARK_SIGNAL_LOW = 0x54,
                    EVENT_IPSOFTWARE_PLIMIT = 0x55,
                    EVENT_IPSOFTWARE_NLIMIT = 0x56,
                    EVENT_IPSOFTWARE_LIMIT = 0x57,
                    EVENT_IPTRIGGER_ENABLE = 0x58,
                    EVENT_IPINT_GEN_SOURCE = 0x59,
                    EVENT_IPINT_GEN_CMDF9 = 0x5A,
                    EVENT_IPPRESETDRIVE_TRI_START = 0x5B,
                    EVENT_IPBUSY_HIGH = 0x5C,
                    EVENT_IPBUSY_LOW = 0x5D,
                    EVENT_IPLINP_START = 0x5E,
                    EVENT_IPLINP_END = 0x5F,
                    EVENT_IPCINP_START = 0x60,
                    EVENT_IPCINP_END = 0x61,
                    EVENT_IPPINP_START = 0x62,
                    EVENT_IPPINP_END = 0x63,
                    EVENT_IPPDATA_Q_EMPTY = 0x64,
                    EVENT_IPS_C_INTERNAL_COMMAND_Q_EMPTY = 0x65,
                    EVENT_IPS_C_INTERNAL_COMMAND_Q_FULL = 0x66,
                    EVENT_IPxSYNC_ACTIVATED = 0x67,
                    EVENT_IPySYNC_ACTIVATED = 0x68,
                    EVENT_IPINTERRUPT_GENERATED = 0x69,
                    EVENT_IPINP_START = 0x6A,
                    EVENT_IPINP_END = 0x6B,
                    EVENT_IPALGEBRIC_RESULT_BIT0 = 0x6C,
                    EVENT_IPALGEBRIC_RESULT_BIT1 = 0x6D,
                    EVENT_IPALGEBRIC_RESULT_BIT2 = 0x6E,
                    EVENT_IPALGEBRIC_RESULT_BIT3 = 0x6F,
                    EVENT_IPALGEBRIC_RESULT_BIT4 = 0x70,
                    EVENT_IPALGEBRIC_RESULT_BIT5 = 0x71,
                    EVENT_IPALGEBRIC_RESULT_BIT6 = 0x72,
                    EVENT_IPALGEBRIC_RESULT_BIT7 = 0x73,
                    EVENT_IPALGEBRIC_RESULT_BIT8 = 0x74,
                    EVENT_IPALGEBRIC_RESULT_BIT9 = 0x75,
                    EVENT_IPALGEBRIC_RESULT_BIT10 = 0x76,
                    EVENT_IPALGEBRIC_RESULT_BIT11 = 0x77,
                    EVENT_IPALGEBRIC_RESULT_BIT12 = 0x78,
                    EVENT_IPALGEBRIC_RESULT_BIT13 = 0x79,
                    EVENT_IPALGEBRIC_RESULT_BIT14 = 0x7A,
                    EVENT_IPALGEBRIC_RESULT_BIT15 = 0x7B,
                    EVENT_IPALGEBRIC_RESULT_BIT16 = 0x7C,
                    EVENT_IPALGEBRIC_RESULT_BIT17 = 0x7D,
                    EVENT_IPALGEBRIC_RESULT_BIT18 = 0x7E,
                    EVENT_IPALGEBRIC_RESULT_BIT19 = 0x7F,
                    EVENT_IPALGEBRIC_RESULT_BIT20 = 0x80,
                    EVENT_IPALGEBRIC_RESULT_BIT21 = 0x81,
                    EVENT_IPALGEBRIC_RESULT_BIT22 = 0x82,
                    EVENT_IPALGEBRIC_RESULT_BIT23 = 0x83,
                    EVENT_IPALGEBRIC_RESULT_BIT24 = 0x84,
                    EVENT_IPALGEBRIC_RESULT_BIT25 = 0x85,
                    EVENT_IPALGEBRIC_RESULT_BIT26 = 0x86,
                    EVENT_IPALGEBRIC_RESULT_BIT27 = 0x87,
                    EVENT_IPALGEBRIC_RESULT_BIT28 = 0x88,
                    EVENT_IPALGEBRIC_RESULT_BIT29 = 0x89,
                    EVENT_IPALGEBRIC_RESULT_BIT30 = 0x8A,
                    EVENT_IPALGEBRIC_RESULT_BIT31 = 0x8B,
                    EVENT_IPCOMPARE_RESULT_BIT0 = 0x8C,
                    EVENT_IPCOMPARE_RESULT_BIT1 = 0x8D,
                    EVENT_IPCOMPARE_RESULT_BIT2 = 0x8E,
                    EVENT_IPCOMPARE_RESULT_BIT3 = 0x8F,
                    EVENT_IPCOMPARE_RESULT_BIT4 = 0x90,
                    EVENT_IPON_INTERPOLATION = 0x91,
                    EVENT_IPON_LINEAR_INTERPOLATION = 0x92,
                    EVENT_IPON_CIRCULAR_INTERPOLATION = 0x93,
                    EVENT_IPON_PATTERN_INTERPOLATION = 0x94,
                    EVENT_IPNONE_95 = 0x95,
                    EVENT_IPL_C_INP_Q_EMPTY = 0x96,
                    EVENT_IPL_C_INP_Q_LESS_4 = 0x97,
                    EVENT_IPP_INP_Q_EMPTY = 0x98,
                    EVENT_IPP_INP_Q_LESS_4 = 0x99,
                    EVENT_IPINTERPOLATION_PAUSED = 0x9A,
                    EVENT_IPP_INP_END_BY_END_PATTERN = 0x9B,
                    EVENT_IPARITHMETIC_DATA_SEL = 0xEE,
                    EVENT_IPEXECUTION_ALWAYS = 0xFF
                };

                public enum QIEVENT
                {
                    EVENT_QINOOP = 0x00,         // No operation.
                    EVENT_QIDRVEND = 0x01,         // Drive end event(inposition function excluded).
                    EVENT_QIDECEL = 0X02,         // Deceleration state.
                    EVENT_QICONST = 0x03,         // Constant speed state.
                    EVENT_QIACCEL = 0X04,         // Acceleration state.
                    EVENT_QICNT1L = 0x05,         // Counter1 < Comparater1 state.
                    EVENT_QICNT1E = 0X06,         // Counter1 = Comparater1 state.
                    EVENT_QICNT1G = 0x07,         // Counter1 > Comparater1 state.
                    EVENT_QICNT1LE = 0x08,         // Counter1 ≤ Comparater1 state.
                    EVENT_QICNT1GE = 0x09,         // Counter1 ≥ Comparater1 state.
                    EVENT_QICNT1EUP = 0x0A,         // Counter1 = Comparater1 event during counting up.
                    EVENT_QICNT1EDN = 0x0B,         // Counter1 = Comparater1 event during counting down.
                    EVENT_QICNT1BND = 0x0C,         // Counter1 is same with boundary value.
                    EVENT_QICNT2L = 0x0D,         // Counter2 < Comparater2 state.
                    EVENT_QICNT2E = 0x0E,         // Counter2 = Comparater2 state.
                    EVENT_QICNT2G = 0x0F,         // Counter2 > Comparater2 state.
                    EVENT_QICNT2LE = 0x10,         // Counter2 ≤ Comparater2 state.
                    EVENT_QICNT2GE = 0x11,         // Counter2 ≥ Comparater2 state.
                    EVENT_QICNT2EUP = 0x12,         // Counter2 = Comparater2 event during counting up.
                    EVENT_QICNT2EDN = 0x13,         // Counter2 = Comparater2 event during counting down.
                    EVENT_QICNT2BND = 0x14,         // Counter2 is same with boundary value.
                    EVENT_QICNT3L = 0x15,         // Counter3 < Comparater3 state.
                    EVENT_QICNT3E = 0x16,         // Counter3 = Comparater3 state.
                    EVENT_QICNT3G = 0x17,         // Counter3 > Comparater3 state.
                    EVENT_QICNT3LE = 0x18,         // Counter3 ≤ Comparater3 state.
                    EVENT_QICNT3GE = 0x19,         // Counter3 ≥ Comparater3 state.
                    EVENT_QICNT3EUP = 0x1A,         // Counter3 = Comparater3 event during counting up.
                    EVENT_QICNT3EDN = 0x1B,         // Counter3 = Comparater3 event during counting down.
                    EVENT_QICNT3BND = 0x1C,         // Counter3 is same with boundary value.
                    EVENT_QICNT4L = 0x1D,         // Counter4 < Comparater4 state.
                    EVENT_QICNT4E = 0x1E,         // Counter4 = Comparater4 state.
                    EVENT_QICNT4G = 0x1F,         // Counter4 > Comparater4 state.
                    EVENT_QICNT4LE = 0x20,         // Counter4 ≤ Comparater4 state.
                    EVENT_QICNT4GE = 0x21,         // Counter4 ≥ Comparater4 state.
                    EVENT_QICNT4EUP = 0x22,         // Counter4 = Comparater4 event during counting up.
                    EVENT_QICNT4EDN = 0x23,         // Counter4 = Comparater4 event during counting down.
                    EVENT_QICNT4BND = 0x24,         // Counter4 is same with boundary value.
                    EVENT_QICNT5L = 0x25,         // Counter5 < Comparater5 state.
                    EVENT_QICNT5E = 0x26,         // Counter5 = Comparater5 state.
                    EVENT_QICNT5G = 0x27,         // Counter5 > Comparater5 state.
                    EVENT_QICNT5LE = 0x28,         // Counter5 ≤ Comparater5 state.
                    EVENT_QICNT5GE = 0x29,         // Counter5 ≥ Comparater5 state.
                    EVENT_QICNT5EUP = 0x2A,         // Counter5 = Comparater5 event during counting up.
                    EVENT_QICNT5EDN = 0x2B,         // Counter5 = Comparater5 event during counting down.
                    EVENT_QICNT5BND = 0x2C,         // Counter5 is same with boundary value.
                    EVENT_QIDEVL = 0x2D,         // DEVIATION value < Comparater4 state.
                    EVENT_QIDEVE = 0x2E,         // DEVIATION value = Comparater4 state.
                    EVENT_QIDEVG = 0x2F,         // DEVIATION value > Comparater4 state.
                    EVENT_QIDEVLE = 0x30,         // DEVIATION value ≤ Comparater4 state.
                    EVENT_QIDEVGE = 0x31,         // DEVIATION value ≥ Comparater4 state.
                    EVENT_QIPELM = 0x32,         // PELM input signal is activated state.
                    EVENT_QINELM = 0x33,         // NELM input signal is activated state.
                    EVENT_QIPSLM = 0x34,         // PSLM input signal is activated state.
                    EVENT_QINSLM = 0x35,         // NSLM input signal is activated state.
                    EVENT_QIALARM = 0x36,         // ALAMR input signal is activated state.
                    EVENT_QIINPOS = 0x37,         // INPOSITION input signal ia activated state.
                    EVENT_QIESTOP = 0x38,         // ESTOP input signal is activated state.
                    EVENT_QIORG = 0x39,         // ORG input signal is activated state.
                    EVENT_QIZ_PHASE = 0x3A,         // Z_PHASE input signal is activated state.
                    EVENT_QIECUP = 0x3B,         // ECUP input signal is high level state.
                    EVENT_QIECDN = 0x3C,         // ECDN input signal is high level state.
                    EVENT_QIEXPP = 0x3D,         // EXPP input signal is high level state.
                    EVENT_QIEXMP = 0x3E,         // EXMP input signal is high level state.
                    EVENT_QISQSTR1 = 0x3F,         // SYNC Start1 input signal is activated state(activated).
                    EVENT_QISQSTR2 = 0x40,         // SYNC Start2 input signal is activated state(activated).
                    EVENT_QISQSTP1 = 0x41,         // SYNC STOP1 input signal is activated state(activated).
                    EVENT_QISQSTP2 = 0x42,         // SYNC STOP2 input signal is activated state(activated).
                    EVENT_QIALARMS = 0x43,         // At least one alarm signal of each axis is activated state.
                    EVENT_QIUIO0 = 0x44,         // UIO0 data is high state.
                    EVENT_QIUIO1 = 0x45,         // UIO1 data is high state.
                    EVENT_QIUIO2 = 0x46,         // UIO2 data is high state.
                    EVENT_QIUIO3 = 0x47,         // UIO3 data is high state.
                    EVENT_QIUIO4 = 0x48,         // UIO4 data is high state.
                    EVENT_QIUIO5 = 0x49,         // UIO5 data is high state.
                    EVENT_QIUIO6 = 0x4A,         // UIO6 data is high state.
                    EVENT_QIUIO7 = 0x4B,         // UIO7 data is high state.
                    EVENT_QIUIO8 = 0x4C,         // UIO8 data is high state.
                    EVENT_QIUIO9 = 0x4D,         // UIO9 data is high state.
                    EVENT_QIUIO10 = 0x4E,         // UIO10 data is high state.
                    EVENT_QIUIO11 = 0x4F,         // UIO11 data is high state.
                    EVENT_QIERC = 0x50,         // ERC output is activated.
                    EVENT_QITRG = 0x51,         // TRIGGER signal is activated.
                    EVENT_QIPREQI0 = 0x52,         // Previous queue data index 0 bit is high state.
                    EVENT_QIPREQI1 = 0x53,         // Previous queue data index 1 bit is high state.
                    EVENT_QIPREQI2 = 0x54,         // Previous queue data index 2 bit is high state.
                    EVENT_QIPREQZ = 0x55,         // Previous queue is empty state.
                    EVENT_QIPREQF = 0x56,         // Previous queue is full state.
                    EVENT_QIMPGE1 = 0x57,         // MPG first stage is overflowed state.
                    EVENT_QIMPGE2 = 0x58,         // MPG second stage is overflowed state.
                    EVENT_QIMPGE3 = 0x59,         // MPG third stage is overflowed state.
                    EVENT_QIMPGERR = 0x5A,         // MPG all state is overflowed state.
                    EVENT_QITRGCNT0 = 0x5B,         // TRIGGER queue index bit 0 is high state.
                    EVENT_QITRGCNT1 = 0x5C,         // TRIGGER queue index bit 1 is high state.
                    EVENT_QITRGCNT2 = 0x5D,         // TRIGGER queue index bit 2 is high state.
                    EVENT_QITRGCNT3 = 0x5E,         // TRIGGER queue index bit 3 is high state.
                    EVENT_QITRGQEPT = 0x5F,         // TRIGGER queue is empty state.
                    EVENT_QITRGQFULL = 0x60,         // TRIGGER queue is full state.
                    EVENT_QIDPAUSE = 0x61,         // Drive paused state.
                    EVENT_QIESTOPEXE = 0x62,         // Emergency stop occurred
                    EVENT_QISSTOPEXE = 0x63,         // Slowdown stop occurred
                    EVENT_QIPLMTSTOP = 0x64,         // Limit stop event occurred during positive driving.
                    EVENT_QINLMTSTOP = 0x65,         // Limit stop event occurred during negative driving.
                    EVENT_QIOPLMTSTOP = 0x66,         // Optional limit stop event occurred during positive driving.
                    EVENT_QIONLMTSTOP = 0x67,         // Optional limit stop event occurred during negative driving.
                    EVENT_QIPSWESTOP = 0x68,         // Software emergency limit stop event occurred.(CW)
                    EVENT_QINSWESTOP = 0x69,         // Software emergency limit stop event occurred.(CCW)
                    EVENT_QIPSWSSTOP = 0x6A,         // Software slowdown limit stop event occurred.(CW)
                    EVENT_QINSWSSTOP = 0x6B,         // Software slowdown limit stop event occurred.(CCW)
                    EVENT_QIALMSTOP = 0x6C,         // Emergency stop event occurred by alarm signal function.
                    EVENT_QIESTOPSTOP = 0x6D,         // Emergency stop event occurred by estop signal function.
                    EVENT_QIESTOPCMD = 0x6E,         // Emergency stop event occurred by command.
                    EVENT_QISSTOPCMD = 0x6F,         // Slowdown stop event occurred by command.
                    EVENT_QIALLSTCMD = 0x70,         // Emergency stop event occurred by all stop command.
                    EVENT_QISYSTOP1 = 0x71,         // SYNC stop1 event occurred.
                    EVENT_QISYSTOP2 = 0x72,         // SYNC stop2 event occurred.
                    EVENT_QIENCODERR = 0x73,         // Encoder input error event occurred.
                    EVENT_QIMPGOVERFLOW = 0x74,         // MPG input error event occurred.
                    EVENT_QIORGOK = 0x75,         // Original drive is executed successfully.
                    EVENT_QISSCHOK = 0x76,         // Signal search drive is executed successfully.
                    EVENT_QIUIO0LOW = 0x77,         // UIO0 data is low state.
                    EVENT_QIUIO1LOW = 0x78,         // UIO1 data is low state.
                    EVENT_QIUIO2LOW = 0x79,         // UIO2 data is low state.
                    EVENT_QIUIO3LOW = 0x7A,         // UIO3 data is low state.
                    EVENT_QIUIO4LOW = 0x7B,         // UIO4 data is low state.
                    EVENT_QIUIO5LOW = 0x7C,         // UIO5 data is low state.
                    EVENT_QIUIO6LOW = 0x7D,         // UIO6 data is low state.
                    EVENT_QIUIO7LOW = 0x7E,         // UIO7 data is low state.
                    EVENT_QIUIO8LOW = 0x7F,         // UIO8 data is low state.
                    EVENT_QIUIO9LOW = 0x80,         // UIO9 data is low state.
                    EVENT_QIUIO10LOW = 0x81,         // UIO10 data is low state.
                    EVENT_QIUIO11LOW = 0x82,         // UIO11 data is low state.
                    EVENT_QIUIO0RISING = 0x83,         // UIO0 rising edge event occurred.
                    EVENT_QIUIO1RISING = 0x84,         // UIO1 rising edge event occurred.
                    EVENT_QIUIO2RISING = 0x85,         // UIO2 rising edge event occurred.
                    EVENT_QIUIO3RISING = 0x86,         // UIO3 rising edge event occurred.
                    EVENT_QIUIO4RISING = 0x87,         // UIO4 rising edge event occurred.
                    EVENT_QIUIO5RISING = 0x88,         // UIO5 rising edge event occurred.
                    EVENT_QIUIO6RISING = 0x89,         // UIO6 rising edge event occurred.
                    EVENT_QIUIO7RISING = 0x8A,         // UIO7 rising edge event occurred.
                    EVENT_QIUIO8RISING = 0x8B,         // UIO8 rising edge event occurred.
                    EVENT_QIUIO9RISING = 0x8C,         // UIO9 rising edge event occurred.
                    EVENT_QIUIO10RISING = 0x8D,         // UIO10 rising edge event occurred.
                    EVENT_QIUIO11RISING = 0x8E,         // UIO11 rising edge event occurred.
                    EVENT_QIUIO0FALLING = 0x8F,         // UIO0 falling edge event occurred.
                    EVENT_QIUIO1FALLING = 0x90,         // UIO1 falling edge event occurred.
                    EVENT_QIUIO2FALLING = 0x91,         // UIO2 falling edge event occurred.
                    EVENT_QIUIO3FALLING = 0x92,         // UIO3 falling edge event occurred.
                    EVENT_QIUIO4FALLING = 0x93,         // UIO4 falling edge event occurred.
                    EVENT_QIUIO5FALLING = 0x94,         // UIO5 falling edge event occurred.
                    EVENT_QIUIO6FALLING = 0x95,         // UIO6 falling edge event occurred.
                    EVENT_QIUIO7FALLING = 0x96,         // UIO7 falling edge event occurred.
                    EVENT_QIUIO8FALLING = 0x97,         // UIO8 falling edge event occurred.
                    EVENT_QIUIO9FALLING = 0x98,         // UIO9 falling edge event occurred.
                    EVENT_QIUIO10FALLING = 0x99,         // UIO10 falling edge event occurred.
                    EVENT_QIUIO11FALLING = 0x9A,         // UIO11 falling edge event occurred.
                    EVENT_QIDRVSTR = 0x9B,         // Drive started.
                    EVENT_QIDNSTR = 0x9C,         // Speed down event occurred.
                    EVENT_QICOSTR = 0x9D,         // Constant speed event occurred.
                    EVENT_QIUPSTR = 0x9E,         // Speed up event occurred.
                    EVENT_QICONTISTR = 0x9F,         // Continuous drive started.
                    EVENT_QIPRESETSTR = 0xA0,         // Preset drive started.
                    EVENT_QIMPGSTR = 0xA1,         // MPG drive started.
                    EVENT_QIORGSTR = 0Xa2,         // Original drive started.
                    EVENT_QISSCHSTR = 0xA3,         // Signal search drive started.
                    EVENT_QIPATHSTR = 0xA4,         // Interpolation drive started.
                    EVENT_QISLAVESTR = 0xA5,         // Slave drive started.
                    EVENT_QICCWSTR = 0xA6,         // CCW direction drive started.
                    EVENT_QIINPWAIT = 0xA7,         // Inposition wait event occurred.
                    EVENT_QILINSTR = 0xA8,         // Linear drive stated.
                    EVENT_QICIRSTR = 0xA9,         // Circular drive started.
                    EVENT_QIDRVENDII = 0xAA,         // Drive stopped.(Inposition state included)
                    EVENT_QIDNEND = 0xAB,         // Speed down end event occurred.
                    EVENT_QICOEND = 0xAC,         // Constant speed end event occurred.
                    EVENT_QIUPEND = 0xAD,         // Speed up end event occurred.
                    EVENT_QICONTIEND = 0xAE,         // Continuous drive ended.
                    EVENT_QIPRESETEND = 0xAF,         // Preset drive ended.
                    EVENT_QIMPGEND = 0xB0,         // MPG drive ended.
                    EVENT_QIORGEND = 0xB1,         // Original drive ended.
                    EVENT_QISSCHEND = 0XB2,         // Signal search drive ended.
                    EVENT_QIPATHEND = 0xB3,         // Interpolation drive ended.
                    EVENT_QISLAVEEND = 0xB4,         // Slave drive ended.
                    EVENT_QICCWEND = 0xB5,         // CCW direction drive ended.
                    EVENT_QIINPEND = 0xB6,         // Escape from Inposition waiting.
                    EVENT_QILINEND = 0xB7,         // Linear drive ended.
                    EVENT_QICIREND = 0xB8,         // Circular drive ended.
                    EVENT_QIBUSY = 0xB9,         // During driving state.
                    EVENT_QINBUSY = 0xBA,         // During not driving state.
                    EVENT_QITMR1EX = 0xBB,         // Timer1 expired event.
                    EVENT_QITMR2EX = 0xBC,         // Timer2 expired event.
                    EVENT_QIDRVENDIII = 0xBD,         // Drive(that interrupt enable bit is set to high) end event 
                    EVENT_QIERROR = 0xBE,         // Error stop occurred.
                    //EVENT_QINOP                           = 0xBF,         // NOP.
                    EVENT_QIALWAYS = 0xFF          // Always Generate.
                };

                //-------------------------------
                // CAMC-QI Script/Caption Define
                //-------------------------------
                public const int QI_SCR_REG1 = 1;
                public const int QI_SCR_REG2 = 2;
                public const int QI_SCR_REG3 = 3;
                public const int QI_SCR_REG4 = 4;
                public const int QI_OPERATION_ONCE_RUN = 0x00000000;    // bit 24 OFF
                public const int QI_OPERATION_CONTINUE_RUN = 0x01000000;    // bit 24 ON
                public const int QI_INPUT_DATA_FROM_SCRIPT_DATA = 0x00000000;    // bit 23 OFF
                public const int QI_INPUT_DATA_FROM_TARGET_REG = 0x00800000;    // bit 23 ON
                //public const int QI_INTERRUPT_GEN_ENABLE        = 0x00000000;    // bit 22 OFF
                //public const int QI_INTERRUPT_GEN_DISABLE       = 0x00400000;    // bit 22 ON
                public const int QI_INTERRUPT_GEN_ENABLE = 0x00400000;    // bit 22 ON
                public const int QI_INTERRUPT_GEN_DISABLE = 0x00000000;    // bit 22 OFF
                public const int QI_OPERATION_EVENT_NONE = 0x00000000;    // bit 21=OFF, 20=OFF        
                //public const int QI_OPERATION_EVENT_OR          = 0x00100000;    // bit 21=OFF, 20=ON
                //public const int QI_OPERATION_EVENT_AND         = 0x00200000;    // bit 21=ON,  20=OF
                public const int QI_OPERATION_EVENT_AND = 0x00100000;    // bit 21=OFF, 20=ON
                public const int QI_OPERATION_EVENT_OR = 0x00200000;    // bit 21=ON,  20=OFF
                public const int QI_OPERATION_EVENT_XOR = 0x00300000;    // bit 21=ON,  20=ON

                public static uint QI_SND_EVENT_AXIS(int Axis)                     // bit 19~18 (00:X, 01:Y, 10:Z, 11:U)
                {
                    return (((uint)Axis % 4) << 18);
                }

                public static uint QI_FST_EVENT_AXIS(int Axis)                     // bit 17~16 (00:X, 01:Y, 10:Z, 11:U)
                {
                    return (((uint)Axis % 4) << 16);
                }

                public static uint QI_OPERATION_EVENT_2(uint Event)                // bit 15..8
                {
                    return ((Event & 0xFF) << 8);
                }

                public static uint QI_OPERATION_EVENT_1(uint Event)                // bit 7..0
                {
                    return (Event & 0xFF);
                }

                public static uint QI_OPERATION_COMMAND(int Command, int Axis)     // bit 7..0 : enum _QISCOMMAND 참조
                {
                    return (uint)((Command & 0xFF) << ((Axis % 4) * 8));
                }
            }

            // 베이스보드 정의
            public enum AXT_BASE_BOARD : uint
            {
                AXT_UNKNOWN = 0x00,     // Unknown Baseboard
                AXT_BIHR = 0x01,     // ISA bus, Half size
                AXT_BIFR = 0x02,     // ISA bus, Full size
                AXT_BPHR = 0x03,     // PCI bus, Half size
                AXT_BPFR = 0x04,     // PCI bus, Full size
                AXT_BV3R = 0x05,     // VME bus, 3U size
                AXT_BV6R = 0x06,     // VME bus, 6U size
                AXT_BC3R = 0x07,     // cPCI bus, 3U size
                AXT_BC6R = 0x08,     // cPCI bus, 6U size
                AXT_FMNSH4D = 0x52,     // ISA bus, Full size, DB-32T, SIO-2V03 * 2
                AXT_PCI_DI64R = 0x43,     // PCI bus, Digital IN 64점
                AXT_PCI_DO64R = 0x53,     // PCI bus, Digital OUT 64점
                AXT_PCI_DB64R = 0x63,     // PCI bus, Digital IN 32점, OUT 32점
                AXT_BPHD = 0x83,     // PCI bus, Half size, DB-32T
                AXT_PCIN404 = 0x84,     // PCI bus, Half size On-Board 4 Axis controller.    
                AXT_PCIN804 = 0x85,     // PCI bus, Half size On-Board 8 Axis controller.
                AXT_PCI_AIO1602HR = 0x93,     // PCI bus, Half size, AI-16ch, AO-2ch AI16HR
            }

            // 모듈 정의                                    
            public enum AXT_MODULE : uint
            {
                AXT_SMC_2V01 = 0x01,     // CAMC-5M, 2 Axis
                AXT_SMC_2V02 = 0x02,     // CAMC-FS, 2 Axis
                AXT_SMC_1V01 = 0x03,     // CAMC-5M, 1 Axis
                AXT_SMC_1V02 = 0x04,     // CAMC-FS, 1 Axis
                AXT_SMC_2V03 = 0x05,     // CAMC-IP, 2 Axis
                AXT_SMC_4V04 = 0x06,     // CAMC-QI, 4 Axis
                AXT_SMC_1V03 = 0x08,     // CAMC-IP, 1 Axis
                AXT_SMC_2V04 = 0x0C,     // CAMC-QI, 2 Axis    
                AXT_SMC_4V51 = 0x33,     // MCX314,  4 Axis
                AXT_SMC_2V53 = 0x35,     // PMD, 2 Axis
                AXT_SMC_2V54 = 0x36,     // MCX312,  2 Axis
                AXT_SIO_DI32 = 0x97,     // Digital IN  32점
                AXT_SIO_DO32P = 0x98,     // Digital OUT 32점
                AXT_SIO_DB32P = 0x99,     // Digital IN 16점 / OUT 16점
                AXT_SIO_DO32T = 0x9E,     // Digital OUT 16점, Power TR 출력
                AXT_SIO_DB32T = 0x9F,     // Digital IN 16점 / OUT 16점, Power TR 출력
                AXT_SIO_AI4RB = 0xA1,     // A1h(161) : AI 4Ch, 12 bit
                AXT_SIO_AO4RB = 0xA2,     // A2h(162) : AO 4Ch, 12 bit
                AXT_SIO_AI16H = 0xA3,     // A3h(163) : AI 4Ch, 16 bit
                AXT_SIO_AO8H = 0xA4,     // A4h(164) : AO 4Ch, 16 bit
                AXT_SIO_AI16HB = 0xA5,     // A5h(165) : AI 16Ch, 16 bit (SIO-AI16HR(input module))
                AXT_SIO_AO2HB = 0xA6,     // A6h(166) : AO 2Ch, 16 bit  (SIO-AI16HR(output module))
                AXT_COM_234R = 0xD3,     // COM-234R
                AXT_COM_484R = 0xD4,     // COM-484R
                AXT_SIO_RPI2 = 0xD5,     // D5h : Pulse counter module(JEPMC-2900)
            }

            public enum AXT_FUNC_RESULT : uint
            {
                AXT_RT_SUCCESS = 0,        // API 함수 수행 성공
                AXT_RT_OPEN_ERROR = 1001,     // 라이브러리 오픈 되지않음
                AXT_RT_OPEN_ALREADY = 1002,     // 라이브러리 오픈 되어있고 사용 중임
                AXT_RT_NOT_OPEN = 1053,     // 라이브러리 초기화 실패
                AXT_RT_NOT_SUPPORT_VERSION = 1054,     // 지원하지않는 하드웨어
                AXT_RT_INVALID_BOARD_NO = 1101,     // 유효하지 않는 보드 번호
                AXT_RT_INVALID_MODULE_POS = 1102,     // 유효하지 않는 모듈 위치
                AXT_RT_INVALID_LEVEL = 1103,     // 유효하지 않는 레벨
                AXT_RT_INVALID_VARIABLE = 1104,     // 유효하지 않는 변수
                AXT_RT_ERROR_VERSION_READ = 1151,     // 라이브러리 버전을 읽을수 없음
                AXT_RT_NETWORK_ERROR = 1152,     // 하드웨어 네트워크 에러
                AXT_RT_NETWORK_LOCK_MISMATCH = 1153,     // 보드 Lock정보와 현재 Scan정보가 일치하지 않음

                AXT_RT_1ST_BELOW_MIN_VALUE = 1160,     // 첫번째 인자값이 최소값보다 더 작음
                AXT_RT_1ST_ABOVE_MAX_VALUE = 1161,     // 첫번째 인자값이 최대값보다 더 큼
                AXT_RT_2ND_BELOW_MIN_VALUE = 1170,     // 두번째 인자값이 최소값보다 더 작음
                AXT_RT_2ND_ABOVE_MAX_VALUE = 1171,     // 두번째 인자값이 최대값보다 더 큼
                AXT_RT_3RD_BELOW_MIN_VALUE = 1180,     // 세번째 인자값이 최소값보다 더 작음
                AXT_RT_3RD_ABOVE_MAX_VALUE = 1181,     // 세번째 인자값이 최대값보다 더 큼
                AXT_RT_4TH_BELOW_MIN_VALUE = 1190,     // 네번째 인자값이 최소값보다 더 작음
                AXT_RT_4TH_ABOVE_MAX_VALUE = 1191,     // 네번째 인자값이 최대값보다 더 큼
                AXT_RT_5TH_BELOW_MIN_VALUE = 1200,     // 다섯번째 인자값이 최소값보다 더 작음
                AXT_RT_5TH_ABOVE_MAX_VALUE = 1201,     // 다섯번째 인자값이 최대값보다 더 큼
                AXT_RT_6TH_BELOW_MIN_VALUE = 1210,     // 여섯번째 인자값이 최소값보다 더 작음 
                AXT_RT_6TH_ABOVE_MAX_VALUE = 1211,     // 여섯번째 인자값이 최대값보다 더 큼
                AXT_RT_7TH_BELOW_MIN_VALUE = 1220,     // 일곱번째 인자값이 최소값보다 더 작음
                AXT_RT_7TH_ABOVE_MAX_VALUE = 1221,     // 일곱번째 인자값이 최대값보다 더 큼
                AXT_RT_8TH_BELOW_MIN_VALUE = 1230,     // 여덟번째 인자값이 최소값보다 더 작음
                AXT_RT_8TH_ABOVE_MAX_VALUE = 1231,     // 여덟번째 인자값이 최대값보다 더 큼
                AXT_RT_9TH_BELOW_MIN_VALUE = 1240,     // 아홉번째 인자값이 최소값보다 더 작음
                AXT_RT_9TH_ABOVE_MAX_VALUE = 1241,     // 아홉번째 인자값이 최대값보다 더 큼
                AXT_RT_10TH_BELOW_MIN_VALUE = 1250,     // 열번째 인자값이 최소값보다 더 작음
                AXT_RT_10TH_ABOVE_MAX_VALUE = 1251,     // 열번째 인자값이 최대값보다 더 큼

                AXT_RT_AIO_OPEN_ERROR = 2001,     // AIO 모듈 오픈실패
                AXT_RT_AIO_NOT_MODULE = 2051,     // AIO 모듈 없음
                AXT_RT_AIO_NOT_EVENT = 2052,     // AIO 이벤트 읽지 못함
                AXT_RT_AIO_INVALID_MODULE_NO = 2101,     // 유효하지않은 AIO모듈
                AXT_RT_AIO_INVALID_CHANNEL_NO = 2102,     // 유효하지않은 AIO채널번호
                AXT_RT_AIO_INVALID_USE = 2106,     // AIO 함수 사용못함
                AXT_RT_AIO_INVALID_TRIGGER_MODE = 2107,     // 유효하지않는 트리거 모드

                AXT_RT_DIO_OPEN_ERROR = 3001,     // DIO 모듈 오픈실패
                AXT_RT_DIO_NOT_MODULE = 3051,     // DIO 모듈 없음
                AXT_RT_DIO_NOT_INTERRUPT = 3052,     // DIO 인터럽트 설정안됨
                AXT_RT_DIO_INVALID_MODULE_NO = 3101,     // 유효하지않는 DIO 모듈 번호
                AXT_RT_DIO_INVALID_OFFSET_NO = 3102,     // 유효하지않는 DIO OFFSET 번호
                AXT_RT_DIO_INVALID_LEVEL = 3103,     // 유효하지않는 DIO 레벨
                AXT_RT_DIO_INVALID_MODE = 3104,     // 유효하지않는 DIO 모드
                AXT_RT_DIO_INVALID_VALUE = 3105,     // 유효하지않는 값 설정
                AXT_RT_DIO_INVALID_USE = 3106,     // DIO 함수 사용못함        

                AXT_RT_MOTION_OPEN_ERROR = 4001,     // 모션 라이브러리 Open 실패
                AXT_RT_MOTION_NOT_MODULE = 4051,     // 시스템에 장착된 모션 모듈이 없음
                AXT_RT_MOTION_NOT_INTERRUPT = 4052,     // 인터럽트 결과 읽기 실패
                AXT_RT_MOTION_NOT_INITIAL_AXIS_NO = 4053,     // 해당 축 모션 초기화 실패
                AXT_RT_MOTION_NOT_IN_CONT_INTERPOL = 4054,     // 연속 보간 구동 중이 아닌 상태에서 연속보간 중지 명령을 수행 하였음
                AXT_RT_MOTION_NOT_PARA_READ = 4055,     // 원점 구동 설정 파라미터 로드 실패
                AXT_RT_MOTION_INVALID_AXIS_NO = 4101,     // 해당 축이 존재하지 않음
                AXT_RT_MOTION_INVALID_METHOD = 4102,     // 해당 축 구동에 필요한 설정이 잘못됨
                AXT_RT_MOTION_INVALID_USE = 4103,     // 'uUse' 인자값이 잘못 설정됨
                AXT_RT_MOTION_INVALID_LEVEL = 4104,     // 'uLevel' 인자값이 잘못 설정됨
                AXT_RT_MOTION_INVALID_BIT_NO = 4105,     // 범용 입출력 해당 비트가 잘못 설정됨
                AXT_RT_MOTION_INVALID_STOP_MODE = 4106,     // 모션 정지 모드 설정값이 잘못됨
                AXT_RT_MOTION_INVALID_TRIGGER_MODE = 4107,     // 트리거 설정 모드가 잘못 설정됨
                AXT_RT_MOTION_INVALID_TRIGGER_LEVEL = 4108,     // 트리거 출력 레벨 설정이 잘못됨
                AXT_RT_MOTION_INVALID_SELECTION = 4109,     // 'uSelection' 인자가 COMMAND 또는 ACTUAL 이외의 값으로 설정되어 있음
                AXT_RT_MOTION_INVALID_TIME = 4110,     // Trigger 출력 시간값이 잘못 설정되어 있음
                AXT_RT_MOTION_INVALID_FILE_LOAD = 4111,     // 모션 설정값이 저장된 파일이 로드가 안됨
                AXT_RT_MOTION_INVALID_FILE_SAVE = 4112,     // 모션 설정값을 저장하는 파일 저장에 실패함
                AXT_RT_MOTION_INVALID_VELOCITY = 4113,     // 모션 구동 속도값이 0으로 설정되어 모션 에러 발생
                AXT_RT_MOTION_INVALID_ACCELTIME = 4114,     // 모션 구동 가속 시간값이 0으로 설정되어 모션 에러 발생
                AXT_RT_MOTION_INVALID_PULSE_VALUE = 4115,     // 모션 단위 설정 시 입력 펄스값이 0보다 작은값으로 설정됨
                AXT_RT_MOTION_INVALID_NODE_NUMBER = 4116,     // 위치나 속도 오버라이드 함수가 모션 정지 중에 실햄됨
                AXT_RT_MOTION_INVALID_TARGET = 4117,     // 다축 모션 정지 원인에 관한 플래그를 반환한다.
                AXT_RT_MOTION_ERROR_IN_NONMOTION = 4151,     // 모션 구동중이어야 되는데 모션 구동중이 아닐 때
                AXT_RT_MOTION_ERROR_IN_MOTION = 4152,     // 모션 구동 중에 다른 모션 구동 함수를 실행함
                AXT_RT_MOTION_ERROR = 4153,     // 다축 구동 정지 함수 실행 중 에러 발생함
                AXT_RT_MOTION_ERROR_GANTRY_ENABLE = 4154,     // 겐트리 enable이 되어있어 모션중일 때 또 겐트리 enable을 눌렀을 때
                AXT_RT_MOTION_ERROR_GANTRY_AXIS = 4155,     // 겐트리 축이 마스터채널(축) 번호(0 ~ (최대축수 - 1))가 잘못 들어갔을 때
                AXT_RT_MOTION_ERROR_MASTER_SERVOON = 4156,     // 마스터 축 서보온이 안되어있을 때
                AXT_RT_MOTION_ERROR_SLAVE_SERVOON = 4157,     // 슬레이브 축 서보온이 안되어있을 때
                AXT_RT_MOTION_INVALID_POSITION = 4158,     // 유효한 위치에 없을 때          
                AXT_RT_ERROR_NOT_SAME_MODULE = 4159,     // 똑 같은 모듈내에 있지 않을경우
                AXT_RT_ERROR_NOT_SAME_BOARD = 4160,     // 똑 같은 보드내에 있지 아닐경우
                AXT_RT_ERROR_NOT_SAME_PRODUCT = 4161,     // 제품이 서로 다를경우
                AXT_RT_NOT_CAPTURED = 4162,     // 위치가 저장되지 않을 때
                AXT_RT_ERROR_NOT_SAME_IC = 4163,     // 같은 칩내에 존재하지않을 때
                AXT_RT_ERROR_NOT_GEARMODE = 4164,     // 기어모드로 변환이 안될 때
                AXT_ERROR_CONTI_INVALID_AXIS_NO = 4165,     // 연속보간 축맵핑 시 유효한 축이 아닐 때
                AXT_ERROR_CONTI_INVALID_MAP_NO = 4166,     // 연속보간 맵핑 시 유효한 맵핑 번호가 아닐 때
                AXT_ERROR_CONTI_EMPTY_MAP_NO = 4167,     // 연속보간 맵핑 번호가 비워 있을 때
                AXT_RT_MOTION_ERROR_CACULATION = 4168,     // 계산상의 오차가 발생했을 때
                AXT_RT_ERROR_MOVE_SENSOR_CHECK = 4169,     // 연속보간 구동전 에러센서가(Alarm, EMG, Limit등) 감지된경우 

                AXT_ERROR_HELICAL_INVALID_AXIS_NO = 4170,     // 헬리컬 축 맵핑 시 유효한 축이 아닐 때
                AXT_ERROR_HELICAL_INVALID_MAP_NO = 4171,     // 헬리컬 맵핑 시 유효한 맵핑 번호가 아닐  때 
                AXT_ERROR_HELICAL_EMPTY_MAP_NO = 4172,     // 헬리컬 멥핑 번호가 비워 있을 때

                AXT_ERROR_SPLINE_INVALID_AXIS_NO = 4180,     // 스플라인 축 맵핑 시 유효한 축이 아닐 때
                AXT_ERROR_SPLINE_INVALID_MAP_NO = 4181,     // 스플라인 맵핑 시 유효한 맵핑 번호가 아닐 때
                AXT_ERROR_SPLINE_EMPTY_MAP_NO = 4182,     // 스플라인 맵핑 번호가 비워있을 때
                AXT_ERROR_SPLINE_NUM_ERROR = 4183,     // 스플라인 점숫자가 부적당할 때
                AXT_RT_MOTION_INTERPOL_VALUE = 4184,     // 보간할 때 입력 값이 잘못넣어졌을 때
                AXT_RT_ERROR_NOT_CONTIBEGIN = 4185,     // 연속보간 할 때 CONTIBEGIN함수를 호출하지 않을 때
                AXT_RT_ERROR_NOT_CONTIEND = 4186,     // 연속보간 할 때 CONTIEND함수를 호출하지 않을 때

                AXT_RT_MOTION_HOME_SEARCHING = 4201,     // 홈을 찾고 있는 중일 때 다른 모션 함수들을 사용할 때
                AXT_RT_MOTION_HOME_ERROR_SEARCHING = 4202,     // 홈을 찾고 있는 중일 때 외부에서 사용자나 혹은 어떤것에 의한  강제로 정지당할 때
                AXT_RT_MOTION_HOME_ERROR_START = 4203,     // 초기화 문제로 홈시작 불가할 때
                AXT_RT_MOTION_HOME_ERROR_GANTRY = 4204,     // 홈을 찾고 있는 중일 때 겐트리 enable 불가할 때
                AXT_RT_MOTION_POSITION_OUTOFBOUND = 4251,     // 설정한 위치값이 설정 최대값보다 크거나 최소값보다 작은값임 
                AXT_RT_MOTION_PROFILE_INVALID = 4252,     // 구동 속도 프로파일 설정이 잘못됨
                AXT_RT_MOTION_VELOCITY_OUTOFBOUND = 4253,     // 구동 속도값이 최대값보다 크게 설정됨
                AXT_RT_MOTION_MOVE_UNIT_IS_ZERO = 4254,     // 구동 단위값이 0으로 설정됨
                AXT_RT_MOTION_SETTING_ERROR = 4255,     // 속도, 가속도, 저크, 프로파일 설정이 잘못됨
                AXT_RT_MOTION_IN_CONT_INTERPOL = 4256,     // 연속 보간 구동 중 구동 시작 또는 재시작 함수를 실행하였음
                AXT_RT_MOTION_DISABLE_TRIGGER = 4257,     // 트리거 출력이 Disable 상태임 
                AXT_RT_MOTION_INVALID_CONT_INDEX = 4258,     // 연속 보간 Index값 설정이 잘못됨
                AXT_RT_MOTION_CONT_QUEUE_FULL = 4259,     // 모션 칩의 연속 보간 큐가 Full 상태임
                AXT_RT_PROTECTED_DURING_SERVOON = 4260,     // 서보 온 되어 있는 상태에서 사용 못 함
                AXT_RT_HW_ACCESS_ERROR = 4261      // 메모리 Read / Write 실패
            }

            public enum AXT_BOOLEAN : uint
            {
                FALSE,
                TRUE
            }

            public enum AXT_LOG_LEVEL : uint
            {
                LEVEL_NONE,
                LEVEL_ERROR,
                LEVEL_RUNSTOP,
                LEVEL_FUNCTION
            }

            public enum AXT_EXISTENCE : uint
            {
                STATUS_NOTEXIST,
                STATUS_EXIST
            }

            public enum AXT_USE : uint
            {
                DISABLE,
                ENABLE
            }

            public enum AXT_AIO_TRIGGER_MODE : uint
            {
                DISABLE_MODE = 0,
                NORMAL_MODE = 1,
                TIMER_MODE = 2,
                EXTERNAL_MODE = 3
            }

            public enum AXT_AIO_FULL_MODE : uint
            {
                NEW_DATA_KEEP,
                CURR_DATA_KEEP
            }

            public enum AXT_AIO_INTERRUPT_MASK : uint
            {
                DATA_EMPTY = 0x01,
                DATA_MANY = 0x02,
                DATA_SMAL = 0x04,
                DATA_FULL = 0x08
            }

            public enum AXT_DIO_EDGE : uint
            {
                DOWN_EDGE,
                UP_EDGE
            }

            public enum AXT_MOTION_STOPMODE : uint
            {
                EMERGENCY_STOP,
                SLOWDOWN_STOP
            }

            public enum AXT_MOTION_EDGE : uint
            {
                SIGNAL_UP_EDGE,
                SIGNAL_DOWN_EDGE
            }

            public enum AXT_MOTION_TRIGGER_MODE : uint
            {
                PERIOD_MODE,
                ABS_POS_MODE
            }

            public enum AXT_MOTION_SELECTION : uint
            {
                COMMAND,
                ACTUAL
            }

            public enum AXT_MOTION_LEVEL_MODE : uint
            {
                LOW,
                HIGH,
                UNUSED,
                USED
            }

            public enum AXT_MOTION_PROFILE_MODE : uint
            {
                SYM_TRAPEZOIDE_MODE,
                ASYM_TRAPEZOIDE_MODE,
                QUASI_S_CURVE_MODE,
                SYM_S_CURVE_MODE,
                ASYM_S_CURVE_MODE
            }
            public enum AXT_MOTION_ABSREL : uint
            {
                POS_ABS_MODE,
                POS_REL_MODE
            }
            public enum AXT_MOTION_SIGNAL_LEVEL : uint
            {
                INACTIVE,
                ACTIVE
            }

            public enum AXT_MOTION_HOME_RESULT : uint
            {
                HOME_SUCCESS = 0x01,
                HOME_SEARCHING = 0x02,
                HOME_ERR_GNT_RANGE = 0x10,
                HOME_ERR_USER_BREAK = 0x11,
                HOME_ERR_VELOCITY = 0x12,
                HOME_ERR_AMP_FAULT = 0x13,
                HOME_ERR_NEG_LIMIT = 0x14,
                HOME_ERR_POS_LIMIT = 0x15,
                HOME_ERR_NOT_DETECT = 0x16,
                HOME_ERR_UNKNOWN = 0xFF
            }

            public enum AXT_MOTION_UNIV_INPUT : uint
            {
                UIO_INP0,
                UIO_INP1,
                UIO_INP2,
                UIO_INP3,
                UIO_INP4,
                UIO_INP5
            }

            public enum AXT_MOTION_UNIV_OUTPUT : uint
            {
                UIO_OUT0,
                UIO_OUT1,
                UIO_OUT2,
                UIO_OUT3,
                UIO_OUT4,
                UIO_OUT5
            }

            public enum AXT_MOTION_DETECT_DOWN_START_POINT : uint
            {
                AutoDetect,
                RestPulse
            }

            public enum AXT_MOTION_PULSE_OUTPUT : uint
            {
                OneHighLowHighm,                // 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
                OneHighHighLow,                 // 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
                OneLowLowHigh,                  // 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
                OneLowHighLow,                  // 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
                TwoCcwCwHigh,                   // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
                TwoCcwCwLow,                    // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
                TwoCwCcwHigh,                   // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
                TwoCwCcwLow,                    // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
                TwoPhase                        // 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
            }

            public enum AXT_MOTION_EXTERNAL_COUNTER_INPUT : uint
            {
                ObverseUpDownMode,              // 정방향 Up/Down
                ObverseSqr1Mode,                // 정방향 1체배
                ObverseSqr2Mode,                // 정방향 2체배
                ObverseSqr4Mode,                // 정방향 4체배
                ReverseUpDownMode,              // 역방향 Up/Down
                ReverseSqr1Mode,                // 역방향 1체배
                ReverseSqr2Mode,                // 역방향 2체배
                ReverseSqr4Mode                 // 역방향 4체배
            }

            public enum AXT_MOTION_ACC_UNIT : uint
            {
                UNIT_SEC2 = 0x0,                // unit/sec2
                SEC = 0x1,                // sec
                RPM_SEC2 = 0x2                 // rpm/sec2
            }

            public enum AXT_MOTION_MOVE_DIR : uint
            {
                DIR_CCW = 0x0,                // 반시계방향
                DIR_CW = 0x1                 // 시계방향
            }

            public enum AXT_MOTION_RADIUS_DISTANCE : uint
            {
                SHORT_DISTANCE = 0x0,          // 짧은 거리의 원호 이동 
                LONG_DISTANCE = 0x1           // 긴 거리의 원호 이동 
            }

            public enum AXT_MOTION_INTERPOLATION_AXIS : uint
            {
                INTERPOLATION_AXIS2q = 0x0,    // 2축을 보간으로 사용할 때
                INTERPOLATION_AXIS3 = 0x1,    // 3축을 보간으로 사용할 때
                INTERPOLATION_AXIS4 = 0x2     // 4축을 보간으로 사용할 때
            }

            public enum AXT_MOTION_CONTISTART_NODE : uint
            {
                CONTI_NODE_VELOCITY = 0x0,           // 속도 지정 보간 모드
                CONTI_NODE_MANUAL = 0x1,           // 노드 가감속 보간 모드
                CONTI_NODE_AUTO = 0x2            // 자동 가감속 보간 모드
            }

            public enum AXT_MOTION_HOME_DETECT : uint
            {
                PosEndLimit = 0x0,           // +Elm(End limit) +방향 리미트 센서 신호
                NegEndLimit = 0x1,           // -Elm(End limit) -방향 리미트 센서 신호
                PosSloLimit = 0x2,           // +Slm(Slow Down limit) 신호 - 사용하지 않음
                NegSloLimit = 0x3,           // -Slm(Slow Down limit) 신호 - 사용하지 않음
                HomeSensor = 0x4,           // IN0(ORG)  원점 센서 신호
                EncodZPhase = 0x5,           // IN1(Z상)  Encoder Z상 신호
                UniInput02 = 0x6,           // IN2(범용) 범용 입력 2번 신호
                UniInput03 = 0x7,           // IN3(범용) 범용 입력 3번 신호
            }

            public enum AXT_MOTION_MPG_INPUT_METHOD : uint
            {
                MPG_DIFF_ONE_PHASE = 0x0,           // MPG 입력 방식 One Phase
                MPG_DIFF_TWO_PHASE_1X = 0x1,           // MPG 입력 방식 TwoPhase1
                MPG_DIFF_TWO_PHASE_2X = 0x2,           // MPG 입력 방식 TwoPhase2
                MPG_DIFF_TWO_PHASE_4X = 0x3,           // MPG 입력 방식 TwoPhase4
                MPG_LEVEL_ONE_PHASE = 0x4,           // MPG 입력 방식 Level One Phase
                MPG_LEVEL_TWO_PHASE_1X = 0x5,           // MPG 입력 방식 Level Two Phase1
                MPG_LEVEL_TWO_PHASE_2X = 0x6,           // MPG 입력 방식 Level Two Phase2
                MPG_LEVEL_TWO_PHASE_4X = 0x7,           // MPG 입력 방식 Level Two Phase4
            }

            public enum AXT_MOTION_SENSOR_INPUT_METHOD : uint
            {
                SENSOR_METHOD1 = 0x0,           // 일반 구동
                SENSOR_METHOD2 = 0x1,           // 센서 신호 검출 전은 저속 구동. 신호 검출 후 일반 구동
                SENSOR_METHOD3 = 0x2            // 저속 구동
            }

            public enum AXT_MOTION_HOME_CRC_SELECT : uint
            {
                CRC_SELECT1 = 0x0,           // 위치클리어 사용않함, 잔여펄스 클리어 사용 안함
                CRC_SELECT2 = 0x1,           // 위치클리어 사용함, 잔여펄스 클리어 사용 안함
                CRC_SELECT3 = 0x2,           // 위치클리어 사용안함, 잔여펄스 클리어 사용함
                CRC_SELECT4 = 0x3            // 위치클리어 사용함, 잔여펄스 클리어 사용함
            }

            public enum AXT_MOTION_IPDETECT_DESTINATION_SIGNAL : uint
            {
                PElmNegativeEdge = 0x0,           // +Elm(End limit) 하강 edge
                NElmNegativeEdge = 0x1,           // -Elm(End limit) 하강 edge
                PSlmNegativeEdge = 0x2,           // +Slm(Slowdown limit) 하강 edge
                NSlmNegativeEdge = 0x3,           // -Slm(Slowdown limit) 하강 edge
                In0DownEdge = 0x4,           // IN0(ORG) 하강 edge
                In1DownEdge = 0x5,           // IN1(Z상) 하강 edge
                In2DownEdge = 0x6,           // IN2(범용) 하강 edge
                In3DownEdge = 0x7,           // IN3(범용) 하강 edge
                PElmPositiveEdge = 0x8,           // +Elm(End limit) 상승 edge
                NElmPositiveEdge = 0x9,           // -Elm(End limit) 상승 edge
                PSlmPositiveEdge = 0xa,           // +Slm(Slowdown limit) 상승 edge
                NSlmPositiveEdge = 0xb,           // -Slm(Slowdown limit) 상승 edge
                In0UpEdge = 0xc,           // IN0(ORG) 상승 edge
                In1UpEdge = 0xd,           // IN1(Z상) 상승 edge
                In2UpEdge = 0xe,           // IN2(범용) 상승 edge
                In3UpEdge = 0xf            // IN3(범용) 상승 edge
            }

            public enum AXT_MOTION_IPEND_STATUS : uint
            {
                IPEND_STATUS_SLM = 0x0001,        // Bit 0, limit 감속정지 신호 입력에 의한 종료
                IPEND_STATUS_ELM = 0x0002,        // Bit 1, limit 급정지 신호 입력에 의한 종료
                IPEND_STATUS_SSTOP_SIGNAL = 0x0004,        // Bit 2, 감속 정지 신호 입력에 의한 종료
                IPEND_STATUS_ESTOP_SIGANL = 0x0008,        // Bit 3, 급정지 신호 입력에 의한 종료
                IPEND_STATUS_SSTOP_COMMAND = 0x0010,        // Bit 4, 감속 정지 명령에 의한 종료
                IPEND_STATUS_ESTOP_COMMAND = 0x0020,        // Bit 5, 급정지 정지 명령에 의한 종료
                IPEND_STATUS_ALARM_SIGNAL = 0x0040,        // Bit 6, Alarm 신호 입력에 희한 종료
                IPEND_STATUS_DATA_ERROR = 0x0080,        // Bit 7, 데이터 설정 에러에 의한 종료
                IPEND_STATUS_DEVIATION_ERROR = 0x0100,        // Bit 8, 탈조 에러에 의한 종료
                IPEND_STATUS_ORIGIN_DETECT = 0x0200,        // Bit 9, 원점 검출에 의한 종료
                IPEND_STATUS_SIGNAL_DETECT = 0x0400,        // Bit 10, 신호 검출에 의한 종료(Signal search-1/2 drive 종료)
                IPEND_STATUS_PRESET_PULSE_DRIVE = 0x0800,        // Bit 11, Preset pulse drive 종료
                IPEND_STATUS_SENSOR_PULSE_DRIVE = 0x1000,        // Bit 12, Sensor pulse drive 종료
                IPEND_STATUS_LIMIT = 0x2000,        // Bit 13, Limit 완전정지에 의한 종료
                IPEND_STATUS_SOFTLIMIT = 0x4000,        // Bit 14, Soft limit에 의한 종료
                IPEND_STATUS_INTERPOLATION_DRIVE = 0x8000         // Bit 15, Soft limit에 의한 종료
            }

            public enum AXT_MOTION_IPDRIVE_STATUS : uint
            {
                IPDRIVE_STATUS_BUSY = 0x00001,       // Bit 0, BUSY(드라이브 구동 중)
                IPDRIVE_STATUS_DOWN = 0x00002,       // Bit 1, DOWN(감속 중)
                IPDRIVE_STATUS_CONST = 0x00004,       // Bit 2, CONST(등속 중)
                IPDRIVE_STATUS_UP = 0x00008,       // Bit 3, UP(가속 중)
                IPDRIVE_STATUS_ICL = 0x00010,       // Bit 4, ICL(내부 위치 카운터 < 내부 위치 카운터 비교값)
                IPDRIVE_STATUS_ICG = 0x00020,       // Bit 5, ICG(내부 위치 카운터 > 내부 위치 카운터 비교값)
                IPDRIVE_STATUS_ECL = 0x00040,       // Bit 6, ECL(외부 위치 카운터 < 외부 위치 카운터 비교값)
                IPDRIVE_STATUS_ECG = 0x00080,       // Bit 7, ECG(외부 위치 카운터 > 외부 위치 카운터 비교값)
                IPDRIVE_STATUS_DRIVE_DIRECTION = 0x00100,       // Bit 8, 드라이브 방향 신호(0=CW/1=CCW)
                IPDRIVE_STATUS_COMMAND_BUSY = 0x00200,       // Bit 9, 명령어 수행중
                IPDRIVE_STATUS_PRESET_DRIVING = 0x00400,       // Bit 10, Preset pulse drive 중
                IPDRIVE_STATUS_CONTINUOUS_DRIVING = 0x00800,       // Bit 11, Continuouse speed drive 중
                IPDRIVE_STATUS_SIGNAL_SEARCH_DRIVING = 0x01000,       // Bit 12, Signal search-1/2 drive 중
                IPDRIVE_STATUS_ORG_SEARCH_DRIVING = 0x02000,       // Bit 13, 원점 검출 drive 중
                IPDRIVE_STATUS_MPG_DRIVING = 0x04000,       // Bit 14, MPG drive 중
                IPDRIVE_STATUS_SENSOR_DRIVING = 0x08000,       // Bit 15, Sensor positioning drive 중
                IPDRIVE_STATUS_L_C_INTERPOLATION = 0x10000,       // Bit 16, 직선/원호 보간 중
                IPDRIVE_STATUS_PATTERN_INTERPOLATION = 0x20000,       // Bit 17, 비트 패턴 보간 중
                IPDRIVE_STATUS_INTERRUPT_BANK1 = 0x40000,       // Bit 18, 인터럽트 bank1에서 발생
                IPDRIVE_STATUS_INTERRUPT_BANK2 = 0x80000        // Bit 19, 인터럽트 bank2에서 발생
            }

            public enum AXT_MOTION_IPINTERRUPT_BANK1 : uint
            {
                IPINTBANK1_DONTUSE = 0x00000000,    // INTERRUT DISABLED.
                IPINTBANK1_DRIVE_END = 0x00000001,    // Bit 0, Drive end(default value : 1).
                IPINTBANK1_ICG = 0x00000002,    // Bit 1, INCNT is greater than INCNTCMP.
                IPINTBANK1_ICE = 0x00000004,    // Bit 2, INCNT is equal with INCNTCMP.
                IPINTBANK1_ICL = 0x00000008,    // Bit 3, INCNT is less than INCNTCMP.
                IPINTBANK1_ECG = 0x00000010,    // Bit 4, EXCNT is greater than EXCNTCMP.
                IPINTBANK1_ECE = 0x00000020,    // Bit 5, EXCNT is equal with EXCNTCMP.
                IPINTBANK1_ECL = 0x00000040,    // Bit 6, EXCNT is less than EXCNTCMP.
                IPINTBANK1_SCRQEMPTY = 0x00000080,    // Bit 7, Script control queue is empty.
                IPINTBANK1_CAPRQEMPTY = 0x00000100,    // Bit 8, Caption result data queue is empty.
                IPINTBANK1_SCRREG1EXE = 0x00000200,    // Bit 9, Script control register-1 command is executed.
                IPINTBANK1_SCRREG2EXE = 0x00000400,    // Bit 10, Script control register-2 command is executed.
                IPINTBANK1_SCRREG3EXE = 0x00000800,    // Bit 11, Script control register-3 command is executed.
                IPINTBANK1_CAPREG1EXE = 0x00001000,    // Bit 12, Caption control register-1 command is executed.
                IPINTBANK1_CAPREG2EXE = 0x00002000,    // Bit 13, Caption control register-2 command is executed.
                IPINTBANK1_CAPREG3EXE = 0x00004000,    // Bit 14, Caption control register-3 command is executed.
                IPINTBANK1_INTGGENCMD = 0x00008000,    // Bit 15, Interrupt generation command is executed(0xFF)
                IPINTBANK1_DOWN = 0x00010000,    // Bit 16, At starting point for deceleration drive.
                IPINTBANK1_CONT = 0x00020000,    // Bit 17, At starting point for constant speed drive.
                IPINTBANK1_UP = 0x00040000,    // Bit 18, At starting point for acceleration drive.
                IPINTBANK1_SIGNALDETECTED = 0x00080000,    // Bit 19, Signal assigned in MODE1 is detected.
                IPINTBANK1_SP23E = 0x00100000,    // Bit 20, Current speed is equal with rate change point RCP23.
                IPINTBANK1_SP12E = 0x00200000,    // Bit 21, Current speed is equal with rate change point RCP12.
                IPINTBANK1_SPE = 0x00400000,    // Bit 22, Current speed is equal with speed comparison data(SPDCMP).
                IPINTBANK1_INCEICM = 0x00800000,    // Bit 23, INTCNT(1'st counter) is equal with ICM(1'st count minus limit data)
                IPINTBANK1_SCRQEXE = 0x01000000,    // Bit 24, Script queue command is executed When SCRCONQ's 30 bit is '1'.
                IPINTBANK1_CAPQEXE = 0x02000000,    // Bit 25, Caption queue command is executed When CAPCONQ's 30 bit is '1'.
                IPINTBANK1_SLM = 0x04000000,    // Bit 26, NSLM/PSLM input signal is activated.
                IPINTBANK1_ELM = 0x08000000,    // Bit 27, NELM/PELM input signal is activated.
                IPINTBANK1_USERDEFINE1 = 0x10000000,    // Bit 28, Selectable interrupt source 0(refer "0xFE" command).
                IPINTBANK1_USERDEFINE2 = 0x20000000,    // Bit 29, Selectable interrupt source 1(refer "0xFE" command).
                IPINTBANK1_USERDEFINE3 = 0x40000000,    // Bit 30, Selectable interrupt source 2(refer "0xFE" command).
                IPINTBANK1_USERDEFINE4 = 0x80000000     // Bit 31, Selectable interrupt source 3(refer "0xFE" command).
            }

            public enum AXT_MOTION_IPINTERRUPT_BANK2 : uint
            {
                IPINTBANK2_DONTUSE = 0x00000000,    // INTERRUT DISABLED.
                IPINTBANK2_L_C_INP_Q_EMPTY = 0x00000001,    // Bit 0, Linear/Circular interpolation parameter queue is empty.
                IPINTBANK2_P_INP_Q_EMPTY = 0x00000002,    // Bit 1, Bit pattern interpolation queue is empty.
                IPINTBANK2_ALARM_ERROR = 0x00000004,    // Bit 2, Alarm input signal is activated.
                IPINTBANK2_INPOSITION = 0x00000008,    // Bit 3, Inposition input signal is activated.
                IPINTBANK2_MARK_SIGNAL_HIGH = 0x00000010,    // Bit 4, Mark input signal is activated.
                IPINTBANK2_SSTOP_SIGNAL = 0x00000020,    // Bit 5, SSTOP input signal is activated.
                IPINTBANK2_ESTOP_SIGNAL = 0x00000040,    // Bit 6, ESTOP input signal is activated.
                IPINTBANK2_SYNC_ACTIVATED = 0x00000080,    // Bit 7, SYNC input signal is activated.
                IPINTBANK2_TRIGGER_ENABLE = 0x00000100,    // Bit 8, Trigger output is activated.
                IPINTBANK2_EXCNTCLR = 0x00000200,    // Bit 9, External(2'nd) counter is cleard by EXCNTCLR setting.
                IPINTBANK2_FSTCOMPARE_RESULT_BIT0 = 0x00000400,    // Bit 10, ALU1's compare result bit 0 is activated.
                IPINTBANK2_FSTCOMPARE_RESULT_BIT1 = 0x00000800,    // Bit 11, ALU1's compare result bit 1 is activated.
                IPINTBANK2_FSTCOMPARE_RESULT_BIT2 = 0x00001000,    // Bit 12, ALU1's compare result bit 2 is activated.
                IPINTBANK2_FSTCOMPARE_RESULT_BIT3 = 0x00002000,    // Bit 13, ALU1's compare result bit 3 is activated.
                IPINTBANK2_FSTCOMPARE_RESULT_BIT4 = 0x00004000,    // Bit 14, ALU1's compare result bit 4 is activated.
                IPINTBANK2_SNDCOMPARE_RESULT_BIT0 = 0x00008000,    // Bit 15, ALU2's compare result bit 0 is activated.
                IPINTBANK2_SNDCOMPARE_RESULT_BIT1 = 0x00010000,    // Bit 16, ALU2's compare result bit 1 is activated.
                IPINTBANK2_SNDCOMPARE_RESULT_BIT2 = 0x00020000,    // Bit 17, ALU2's compare result bit 2 is activated.
                IPINTBANK2_SNDCOMPARE_RESULT_BIT3 = 0x00040000,    // Bit 18, ALU2's compare result bit 3 is activated.
                IPINTBANK2_SNDCOMPARE_RESULT_BIT4 = 0x00080000,    // Bit 19, ALU2's compare result bit 4 is activated.
                IPINTBANK2_L_C_INP_Q_LESS_4 = 0x00100000,    // Bit 20, Linear/Circular interpolation parameter queue is less than 4.
                IPINTBANK2_P_INP_Q_LESS_4 = 0x00200000,    // Bit 21, Pattern interpolation parameter queue is less than 4.
                IPINTBANK2_XSYNC_ACTIVATED = 0x00400000,    // Bit 22, X axis sync input signal is activated.
                IPINTBANK2_YSYNC_ACTIVATED = 0x00800000,    // Bit 23, Y axis sync input siangl is activated.
                IPINTBANK2_P_INP_END_BY_END_PATTERN = 0x01000000     // Bit 24, Bit pattern interpolation is terminated by end pattern.
                //IPINTBANK2_                          = 0x02000000,    // Bit 25, Don't care.
                //IPINTBANK2_                          = 0x04000000,    // Bit 26, Don't care.
                //IPINTBANK2_                          = 0x08000000,    // Bit 27, Don't care.
                //IPINTBANK2_                          = 0x10000000,    // Bit 28, Don't care.
                //IPINTBANK2_                          = 0x20000000,    // Bit 29, Don't care.
                //IPINTBANK2_                          = 0x40000000,    // Bit 30, Don't care.
                //IPINTBANK2_                          = 0x80000000     // Bit 31, Don't care.
            }

            public enum AXT_MOTION_IPMECHANICAL_SIGNAL : uint
            {
                IPMECHANICAL_PELM_LEVEL = 0x0001,        // Bit 0, +Limit 급정지 신호가 액티브 됨
                IPMECHANICAL_NELM_LEVEL = 0x0002,        // Bit 1, -Limit 급정지 신호 액티브 됨
                IPMECHANICAL_PSLM_LEVEL = 0x0004,        // Bit 2, +limit 감속정지 신호 액티브 됨
                IPMECHANICAL_NSLM_LEVEL = 0x0008,        // Bit 3, -limit 감속정지 신호 액티브 됨
                IPMECHANICAL_ALARM_LEVEL = 0x0010,        // Bit 4, Alarm 신호 액티브 됨
                IPMECHANICAL_INP_LEVEL = 0x0020,        // Bit 5, Inposition 신호 액티브 됨
                IPMECHANICAL_ENC_DOWN_LEVEL = 0x0040,        // Bit 6, 엔코더 DOWN(B상) 신호 입력 Level
                IPMECHANICAL_ENC_UP_LEVEL = 0x0080,        // Bit 7, 엔코더 UP(A상) 신호 입력 Level
                IPMECHANICAL_EXMP_LEVEL = 0x0100,        // Bit 8, EXMP 신호 입력 Level
                IPMECHANICAL_EXPP_LEVEL = 0x0200,        // Bit 9, EXPP 신호 입력 Level
                IPMECHANICAL_MARK_LEVEL = 0x0400,        // Bit 10, MARK# 신호 액티브 됨
                IPMECHANICAL_SSTOP_LEVEL = 0x0800,        // Bit 11, SSTOP 신호 액티브 됨
                IPMECHANICAL_ESTOP_LEVEL = 0x1000,        // Bit 12, ESTOP 신호 액티브 됨
                IPMECHANICAL_SYNC_LEVEL = 0x2000,        // Bit 13, SYNC 신호 입력 Level
                IPMECHANICAL_MODE8_16_LEVEL = 0x4000         // Bit 14, MODE8_16 신호 입력 Level
            }


            public enum AXT_MOTION_QIDETECT_DESTINATION_SIGNAL : uint
            {
                Signal_PosEndLimit = 0x0,           // +Elm(End limit) +방향 리미트 센서 신호
                Signal_NegEndLimit = 0x1,           // -Elm(End limit) -방향 리미트 센서 신호
                Signal_PosSloLimit = 0x2,           // +Slm(Slow Down limit) 신호 - 사용하지 않음
                Signal_NegSloLimit = 0x3,           // -Slm(Slow Down limit) 신호 - 사용하지 않음
                Signal_HomeSensor = 0x4,           // IN0(ORG)  원점 센서 신호
                Signal_EncodZPhase = 0x5,           // IN1(Z상)  Encoder Z상 신호
                Signal_UniInput02 = 0x6,           // IN2(범용) 범용 입력 2번 신호
                Signal_UniInput03 = 0x7            // IN3(범용) 범용 입력 3번 신호
            }

            public enum AXT_MOTION_QIMECHANICAL_SIGNAL : uint
            {
                QIMECHANICAL_PELM_LEVEL = 0x00001,       // Bit 0, +Limit 급정지 신호 현재 상태
                QIMECHANICAL_NELM_LEVEL = 0x00002,       // Bit 1, -Limit 급정지 신호 현재 상태
                QIMECHANICAL_PSLM_LEVEL = 0x00004,       // Bit 2, +limit 감속정지 현재 상태.
                QIMECHANICAL_NSLM_LEVEL = 0x00008,       // Bit 3, -limit 감속정지 현재 상태
                QIMECHANICAL_ALARM_LEVEL = 0x00010,       // Bit 4, Alarm 신호 신호 현재 상태
                QIMECHANICAL_INP_LEVEL = 0x00020,       // Bit 5, Inposition 신호 현재 상태
                QIMECHANICAL_ESTOP_LEVEL = 0x00040,       // Bit 6, 비상 정지 신호(ESTOP) 현재 상태.
                QIMECHANICAL_ORG_LEVEL = 0x00080,       // Bit 7, 원점 신호 헌재 상태
                QIMECHANICAL_ZPHASE_LEVEL = 0x00100,       // Bit 8, Z 상 입력 신호 현재 상태
                QIMECHANICAL_ECUP_LEVEL = 0x00200,       // Bit 9, ECUP 터미널 신호 상태.
                QIMECHANICAL_ECDN_LEVEL = 0x00400,       // Bit 10, ECDN 터미널 신호 상태.
                QIMECHANICAL_EXPP_LEVEL = 0x00800,       // Bit 11, EXPP 터미널 신호 상태
                QIMECHANICAL_EXMP_LEVEL = 0x01000,       // Bit 12, EXMP 터미널 신호 상태
                QIMECHANICAL_SQSTR1_LEVEL = 0x02000,       // Bit 13, SQSTR1 터미널 신호 상태
                QIMECHANICAL_SQSTR2_LEVEL = 0x04000,       // Bit 14, SQSTR2 터미널 신호 상태
                QIMECHANICAL_SQSTP1_LEVEL = 0x08000,       // Bit 15, SQSTP1 터미널 신호 상태
                QIMECHANICAL_SQSTP2_LEVEL = 0x10000,       // Bit 16, SQSTP2 터미널 신호 상태
                QIMECHANICAL_MODE_LEVEL = 0x20000        // Bit 17, MODE 터미널 신호 상태.
            }

            public enum AXT_MOTION_QIEND_STATUS : uint
            {
                QIEND_STATUS_0 = 0x00000001,    // Bit 0, 정방향 리미트 신호(PELM)에 의한 종료
                QIEND_STATUS_1 = 0x00000002,    // Bit 1, 역방향 리미트 신호(NELM)에 의한 종료
                QIEND_STATUS_2 = 0x00000004,    // Bit 2, 정방향 부가 리미트 신호(PSLM)에 의한 구동 종료
                QIEND_STATUS_3 = 0x00000008,    // Bit 3, 역방향 부가 리미트 신호(NSLM)에 의한 구동 종료
                QIEND_STATUS_4 = 0x00000010,    // Bit 4, 정방향 소프트 리미트 급정지 기능에 의한 구동 종료
                QIEND_STATUS_5 = 0x00000020,    // Bit 5, 역방향 소프트 리미트 급정지 기능에 의한 구동 종료
                QIEND_STATUS_6 = 0x00000040,    // Bit 6, 정방향 소프트 리미트 감속정지 기능에 의한 구동 종료
                QIEND_STATUS_7 = 0x00000080,    // Bit 7, 역방향 소프트 리미트 감속정지 기능에 의한 구동 종료
                QIEND_STATUS_8 = 0x00000100,    // Bit 8, 서보 알람 기능에 의한 구동 종료.
                QIEND_STATUS_9 = 0x00000200,    // Bit 9, 비상 정지 신호 입력에 의한 구동 종료.
                QIEND_STATUS_10 = 0x00000400,    // Bit 10, 급 정지 명령에 의한 구동 종료.
                QIEND_STATUS_11 = 0x00000800,    // Bit 11, 감속 정지 명령에 의한 구동 종료.
                QIEND_STATUS_12 = 0x00001000,    // Bit 12, 전축 급정지 명령에 의한 구동 종료
                QIEND_STATUS_13 = 0x00002000,    // Bit 13, 동기 정지 기능 #1(SQSTP1)에 의한 구동 종료.
                QIEND_STATUS_14 = 0x00004000,    // Bit 14, 동기 정지 기능 #2(SQSTP2)에 의한 구동 종료.
                QIEND_STATUS_15 = 0x00008000,    // Bit 15, 인코더 입력(ECUP,ECDN) 오류 발생
                QIEND_STATUS_16 = 0x00010000,    // Bit 16, MPG 입력(EXPP,EXMP) 오류 발생
                QIEND_STATUS_17 = 0x00020000,    // Bit 17, 원점 검색 성공 종료.
                QIEND_STATUS_18 = 0x00040000,    // Bit 18, 신호 검색 성공 종료.
                QIEND_STATUS_19 = 0x00080000,    // Bit 19, 보간 데이터 이상으로 구동 종료.
                QIEND_STATUS_20 = 0x00100000,    // Bit 20, 비정상 구동 정지발생.
                QIEND_STATUS_21 = 0x00200000,    // Bit 21, MPG 기능 블록 펄스 버퍼 오버플로우 발생
                QIEND_STATUS_22 = 0x00400000,    // Bit 22, DON'CARE
                QIEND_STATUS_23 = 0x00800000,    // Bit 23, DON'CARE
                QIEND_STATUS_24 = 0x01000000,    // Bit 24, DON'CARE
                QIEND_STATUS_25 = 0x02000000,    // Bit 25, DON'CARE
                QIEND_STATUS_26 = 0x04000000,    // Bit 26, DON'CARE
                QIEND_STATUS_27 = 0x08000000,    // Bit 27, DON'CARE
                QIEND_STATUS_28 = 0x10000000,    // Bit 28, 현재/마지막 구동 드라이브 방향
                QIEND_STATUS_29 = 0x20000000,    // Bit 29, 잔여 펄스 제거 신호 출력 중.
                QIEND_STATUS_30 = 0x40000000,    // Bit 30, 비정상 구동 정지 원인 상태
                QIEND_STATUS_31 = 0x80000000     // Bit 31, 보간 드라이브 데이타 오류 상태.
            }

            public enum AXT_MOTION_QIDRIVE_STATUS : uint
            {
                QIDRIVE_STATUS_0 = 0x0000001,     // Bit 0, BUSY(드라이브 구동 중)
                QIDRIVE_STATUS_1 = 0x0000002,     // Bit 1, DOWN(감속 중)
                QIDRIVE_STATUS_2 = 0x0000004,     // Bit 2, CONST(등속 중)
                QIDRIVE_STATUS_3 = 0x0000008,     // Bit 3, UP(가속 중)
                QIDRIVE_STATUS_4 = 0x0000010,     // Bit 4, 연속 드라이브 구동 중
                QIDRIVE_STATUS_5 = 0x0000020,     // Bit 5, 지정 거리 드라이브 구동 중
                QIDRIVE_STATUS_6 = 0x0000040,     // Bit 6, MPG 드라이브 구동 중
                QIDRIVE_STATUS_7 = 0x0000080,     // Bit 7, 원점검색 드라이브 구동중
                QIDRIVE_STATUS_8 = 0x0000100,     // Bit 8, 신호 검색 드라이브 구동 중
                QIDRIVE_STATUS_9 = 0x0000200,     // Bit 9, 보간 드라이브 구동 중
                QIDRIVE_STATUS_10 = 0x0000400,     // Bit 10, Slave 드라이브 구동중
                QIDRIVE_STATUS_11 = 0x0000800,     // Bit 11, 현재 구동 드라이브 방향(보간 드라이브에서는 표시 정보 다름)
                QIDRIVE_STATUS_12 = 0x0001000,     // Bit 12, 펄스 출력후 서보위치 완료 신호 대기중.
                QIDRIVE_STATUS_13 = 0x0002000,     // Bit 13, 직선 보간 드라이브 구동중.
                QIDRIVE_STATUS_14 = 0x0004000,     // Bit 14, 원호 보간 드라이브 구동중.
                QIDRIVE_STATUS_15 = 0x0008000,     // Bit 15, 펄스 출력 중.
                QIDRIVE_STATUS_16 = 0x0010000,     // Bit 16, 구동 예약 데이터 개수(처음)(0-7)
                QIDRIVE_STATUS_17 = 0x0020000,     // Bit 17, 구동 예약 데이터 개수(중간)(0-7)
                QIDRIVE_STATUS_18 = 0x0040000,     // Bit 18, 구동 예약 데이터 갯수(끝)(0-7)
                QIDRIVE_STATUS_19 = 0x0080000,     // Bit 19, 구동 예약 Queue 비어 있음.
                QIDRIVE_STATUS_20 = 0x0100000,     // Bit 20, 구동 예약 Queue 가득 찲
                QIDRIVE_STATUS_21 = 0x0200000,     // Bit 21, 현재 구동 드라이브의 속도 모드(처음)
                QIDRIVE_STATUS_22 = 0x0400000,     // Bit 22, 현재 구동 드라이브의 속도 모드(끝)
                QIDRIVE_STATUS_23 = 0x0800000,     // Bit 23, MPG 버퍼 #1 Full
                QIDRIVE_STATUS_24 = 0x1000000,     // Bit 24, MPG 버퍼 #2 Full
                QIDRIVE_STATUS_25 = 0x2000000,     // Bit 25, MPG 버퍼 #3 Full
                QIDRIVE_STATUS_26 = 0x4000000      // Bit 26, MPG 버퍼 데이터 OverFlow
            }

            public enum AXT_MOTION_QIINTERRUPT_BANK1 : uint
            {
                QIINTBANK1_DISABLE = 0x00000000,    // INTERRUT DISABLED.
                QIINTBANK1_0 = 0x00000001,    // Bit 0,  인터럽트 발생 사용 설정된 구동 종료시.
                QIINTBANK1_1 = 0x00000002,    // Bit 1,  구동 종료시
                QIINTBANK1_2 = 0x00000004,    // Bit 2,  구동 시작시.
                QIINTBANK1_3 = 0x00000008,    // Bit 3,  카운터 #1 < 비교기 #1 이벤트 발생
                QIINTBANK1_4 = 0x00000010,    // Bit 4,  카운터 #1 = 비교기 #1 이벤트 발생
                QIINTBANK1_5 = 0x00000020,    // Bit 5,  카운터 #1 > 비교기 #1 이벤트 발생
                QIINTBANK1_6 = 0x00000040,    // Bit 6,  카운터 #2 < 비교기 #2 이벤트 발생
                QIINTBANK1_7 = 0x00000080,    // Bit 7,  카운터 #2 = 비교기 #2 이벤트 발생
                QIINTBANK1_8 = 0x00000100,    // Bit 8,  카운터 #2 > 비교기 #2 이벤트 발생
                QIINTBANK1_9 = 0x00000200,    // Bit 9,  카운터 #3 < 비교기 #3 이벤트 발생
                QIINTBANK1_10 = 0x00000400,    // Bit 10, 카운터 #3 = 비교기 #3 이벤트 발생
                QIINTBANK1_11 = 0x00000800,    // Bit 11, 카운터 #3 > 비교기 #3 이벤트 발생
                QIINTBANK1_12 = 0x00001000,    // Bit 12, 카운터 #4 < 비교기 #4 이벤트 발생
                QIINTBANK1_13 = 0x00002000,    // Bit 13, 카운터 #4 = 비교기 #4 이벤트 발생
                QIINTBANK1_14 = 0x00004000,    // Bit 14, 카운터 #4 < 비교기 #4 이벤트 발생
                QIINTBANK1_15 = 0x00008000,    // Bit 15, 카운터 #5 < 비교기 #5 이벤트 발생
                QIINTBANK1_16 = 0x00010000,    // Bit 16, 카운터 #5 = 비교기 #5 이벤트 발생
                QIINTBANK1_17 = 0x00020000,    // Bit 17, 카운터 #5 > 비교기 #5 이벤트 발생
                QIINTBANK1_18 = 0x00040000,    // Bit 18, 타이머 #1 이벤트 발생.
                QIINTBANK1_19 = 0x00080000,    // Bit 19, 타이머 #2 이벤트 발생.
                QIINTBANK1_20 = 0x00100000,    // Bit 20, 구동 예약 설정 Queue 비워짐.
                QIINTBANK1_21 = 0x00200000,    // Bit 21, 구동 예약 설정 Queue 가득찲
                QIINTBANK1_22 = 0x00400000,    // Bit 22, 트리거 발생거리 주기/절대위치 Queue 비워짐.
                QIINTBANK1_23 = 0x00800000,    // Bit 23, 트리거 발생거리 주기/절대위치 Queue 가득찲
                QIINTBANK1_24 = 0x01000000,    // Bit 24, 트리거 신호 발생 이벤트
                QIINTBANK1_25 = 0x02000000,    // Bit 25, 스크립트 #1 명령어 예약 설정 Queue 비워짐.
                QIINTBANK1_26 = 0x04000000,    // Bit 26, 스크립트 #2 명령어 예약 설정 Queue 비워짐.
                QIINTBANK1_27 = 0x08000000,    // Bit 27, 스크립트 #3 명령어 예약 설정 레지스터 실행되어 초기화 됨.
                QIINTBANK1_28 = 0x10000000,    // Bit 28, 스크립트 #4 명령어 예약 설정 레지스터 실행되어 초기화 됨.
                QIINTBANK1_29 = 0x20000000,    // Bit 29, 서보 알람신호 인가됨.
                QIINTBANK1_30 = 0x40000000,    // Bit 30, |CNT1| - |CNT2| >= |CNT4| 이벤트 발생.
                QIINTBANK1_31 = 0x80000000     // Bit 31, 인터럽트 발생 명령어|INTGEN| 실행.
            }

            public enum AXT_MOTION_QIINTERRUPT_BANK2 : uint
            {
                QIINTBANK2_DISABLE = 0x00000000,    // INTERRUT DISABLED.
                QIINTBANK2_0 = 0x00000001,    // Bit 0,  스크립트 #1 읽기 명령 결과 Queue 가 가득찲.
                QIINTBANK2_1 = 0x00000002,    // Bit 1,  스크립트 #2 읽기 명령 결과 Queue 가 가득찲.
                QIINTBANK2_2 = 0x00000004,    // Bit 2,  스크립트 #3 읽기 명령 결과 레지스터가 새로운 데이터로 갱신됨.
                QIINTBANK2_3 = 0x00000008,    // Bit 3,  스크립트 #4 읽기 명령 결과 레지스터가 새로운 데이터로 갱신됨.
                QIINTBANK2_4 = 0x00000010,    // Bit 4,  스크립트 #1 의 예약 명령어 중 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
                QIINTBANK2_5 = 0x00000020,    // Bit 5,  스크립트 #2 의 예약 명령어 중 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
                QIINTBANK2_6 = 0x00000040,    // Bit 6,  스크립트 #3 의 예약 명령어 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
                QIINTBANK2_7 = 0x00000080,    // Bit 7,  스크립트 #4 의 예약 명령어 실행 시 인터럽트 발생으로 설정된 명령어 실행됨.
                QIINTBANK2_8 = 0x00000100,    // Bit 8,  구동 시작
                QIINTBANK2_9 = 0x00000200,    // Bit 9,  서보 위치 결정 완료(Inposition)기능을 사용한 구동,종료 조건 발생.
                QIINTBANK2_10 = 0x00000400,    // Bit 10, 이벤트 카운터로 동작 시 사용할 이벤트 선택 #1 조건 발생.
                QIINTBANK2_11 = 0x00000800,    // Bit 11, 이벤트 카운터로 동작 시 사용할 이벤트 선택 #2 조건 발생.
                QIINTBANK2_12 = 0x00001000,    // Bit 12, SQSTR1 신호 인가 됨.
                QIINTBANK2_13 = 0x00002000,    // Bit 13, SQSTR2 신호 인가 됨.
                QIINTBANK2_14 = 0x00004000,    // Bit 14, UIO0 터미널 신호가 '1'로 변함.
                QIINTBANK2_15 = 0x00008000,    // Bit 15, UIO1 터미널 신호가 '1'로 변함.
                QIINTBANK2_16 = 0x00010000,    // Bit 16, UIO2 터미널 신호가 '1'로 변함.
                QIINTBANK2_17 = 0x00020000,    // Bit 17, UIO3 터미널 신호가 '1'로 변함.
                QIINTBANK2_18 = 0x00040000,    // Bit 18, UIO4 터미널 신호가 '1'로 변함.
                QIINTBANK2_19 = 0x00080000,    // Bit 19, UIO5 터미널 신호가 '1'로 변함.
                QIINTBANK2_20 = 0x00100000,    // Bit 20, UIO6 터미널 신호가 '1'로 변함.
                QIINTBANK2_21 = 0x00200000,    // Bit 21, UIO7 터미널 신호가 '1'로 변함.
                QIINTBANK2_22 = 0x00400000,    // Bit 22, UIO8 터미널 신호가 '1'로 변함.
                QIINTBANK2_23 = 0x00800000,    // Bit 23, UIO9 터미널 신호가 '1'로 변함.
                QIINTBANK2_24 = 0x01000000,    // Bit 24, UIO10 터미널 신호가 '1'로 변함.
                QIINTBANK2_25 = 0x02000000,    // Bit 25, UIO11 터미널 신호가 '1'로 변함.
                QIINTBANK2_26 = 0x04000000,    // Bit 26, 오류 정지 조건(LMT, ESTOP, STOP, ESTOP, CMD, ALARM) 발생.
                QIINTBANK2_27 = 0x08000000,    // Bit 27, 보간 중 데이터 설정 오류 발생.
                QIINTBANK2_28 = 0x10000000,    // Bit 28, Don't Care
                QIINTBANK2_29 = 0x20000000,    // Bit 29, 리미트 신호(PELM, NELM)신호가 입력 됨.
                QIINTBANK2_30 = 0x40000000,    // Bit 30, 부가 리미트 신호(PSLM, NSLM)신호가 입력 됨.
                QIINTBANK2_31 = 0x80000000     // Bit 31, 비상 정지 신호(ESTOP)신호가 입력됨.
            }
            public enum AXT_EVENT : uint
            {
                WM_USER = 0x0400,
                WM_AXL_INTERRUPT = (WM_USER + 1001)
            }

            public enum AXT_NETWORK_STATUS : uint
            {
                NET_STATUS_DISCONNECTED = 1,
                NET_STATUS_LOCK_MISMATCH = 5,
                NET_STATUS_CONNECTED = 6
            }

            public enum AXT_AIO_FIFO_STATUS : uint
            {
                FIFO_DATA_EXIST = 0,
                FIFO_DATA_EMPTY = 1,
                FIFO_DATA_HALF = 2,
                FIFO_DATA_FULL = 6
            }

            public enum AXT_AIO_EXTERNAL_STATUS : uint
            {
                EXTERNAL_DATA_DONE = 0,
                EXTERNAL_DATA_FINE = 1,
                EXTERNAL_DATA_HALF = 2,
                EXTERNAL_DATA_FULL = 3,
                EXTERNAL_COMPLETE = 4
            }

            public enum AXT_MOTION_OVERRIDE_MODE : uint
            {
                OVERRIDE_POS_START = 0,
                OVERRIDE_POS_END = 1
            }

            public enum AXT_MOTION_PROFILE_PRIORITY : uint
            {
                PRIORITY_VELOCITY = 0,
                PRIORITY_ACCELTIME = 1
            }

            public class CAXHS
            {
                public delegate void AXT_INTERRUPT_PROC(int nActiveNo, uint uFlag);

                public readonly static uint WM_USER = 0x0400;
                public readonly static uint WM_AXL_INTERRUPT = (WM_USER + 1001);

                public static int AXIS_EVN(int nAxisNo)
                {
                    nAxisNo = (nAxisNo - (nAxisNo % 2));                // 쌍을 이루는 축의 짝수축을 찾음

                    return nAxisNo;
                }

                public static int AXIS_ODD(int nAxisNo)
                {
                    nAxisNo = (nAxisNo + ((nAxisNo + 1) % 2));          // 쌍을 이루는 축의 홀수축을 찾음

                    return nAxisNo;

                }

                public static int AXIS_QUR(int nAxisNo)
                {
                    nAxisNo = (nAxisNo % 4);                            // 쌍을 이루는 축의 홀수축을 찾음

                    return nAxisNo;
                }

                public static int AXIS_N04(int nAxisNo, int nPos)
                {
                    nAxisNo = (((nAxisNo / 4) * 4) + nPos);             // 한 칩의 축 위치로 변경(0~3)

                    return nAxisNo;
                }

                public static int AXIS_N01(int nAxisNo)
                {
                    nAxisNo = ((nAxisNo % 4) >> 2);                     // 0, 1축을 0으로 2, 3축을 1로 변경

                    return nAxisNo;
                }

                public static int AXIS_N02(int nAxisNo)
                {
                    nAxisNo = ((nAxisNo % 4) % 2);                     // 0, 2축을 0으로 1, 3축을 1로 변경

                    return nAxisNo;
                }

                public static int m_SendAxis = 0;          // 현재 축번호

            }

            public class CAXL
            {
                //========== 라이브러리 초기화 =================================================================================

                // 라이브러리 초기화
                [DllImport("AXL.dll")]
                public static extern uint AxlOpen(int lIrqNo);
                // 라이브러리 초기화시 하드웨어 칩에 리셋을 하지 않음.
                [DllImport("AXL.dll")]
                public static extern uint AxlOpenNoReset(uint lIrqNo);
                // 라이브러리 사용을 종료
                [DllImport("AXL.dll")]
                public static extern int AxlClose();
                // 라이브러리가 초기화 되어 있는 지 확인
                [DllImport("AXL.dll")]
                public static extern int AxlIsOpened();

                // 인터럽트를 사용한다.
                [DllImport("AXL.dll")]
                public static extern uint AxlInterruptEnable();
                // 인터럽트를 사용안한다.
                [DllImport("AXL.dll")]
                public static extern uint AxlInterruptDisable();

                //========== 라이브러리 및 베이스 보드 정보 =================================================================================

                // 등록된 베이스 보드의 개수 확인
                [DllImport("AXL.dll")]
                public static extern uint AxlGetBoardCount(ref int lpBoardCount);
                // 라이브러리 버전 확인
                [DllImport("AXL.dll")]
                public static extern uint AxlGetLibVersion(ref char szVersion);

                //========= 로그 레벨 =================================================================================

                // EzSpy에 출력할 메시지 레벨 설정
                // uLevel : 0 - 3 설정
                // LEVEL_NONE(0)    : 모든 메시지를 출력하지 않는다.
                // LEVEL_ERROR(1)   : 에러가 발생한 메시지만 출력한다.
                // LEVEL_RUNSTOP(2) : 모션에서 Run / Stop 관련 메시지를 출력한다.
                // LEVEL_FUNCTION(3): 모든 메시지를 출력한다.
                [DllImport("AXL.dll")]
                public static extern uint AxlSetLogLevel(uint uLevel);
                // EzSpy에 출력할 메시지 레벨 확인
                [DllImport("AXL.dll")]
                public static extern uint AxlGetLogLevel(ref uint upLevel);
            }

            public class CAXA
            {
                //========== 보드 및 모듈 정보 확인 함수 =============================================================
                //AIO 모듈이 있는지 확인한다    
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoIsAIOModule(ref uint upStatus);

                //모듈 No를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetModuleNo(int lBoardNo, int lModulePos, ref int lpModuleNo);

                //AIO 모듈의 개수를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetModuleCount(ref int lpModuleCount);

                //지정한 모듈의 입력 채널 수를 확인한다
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetInputCount(int lModuleNo, ref int lpCount);

                //지정한 모듈의 출력 채널 수를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetOutputCount(int lModuleNo, ref int lpCount);

                //지정한 모듈의 첫 번째 채널 번호를 확인한다.(입력 전용,출력 전용 모듈용)
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetChannelNoOfModuleNo(int lModuleNo, ref int lpChannelNo);

                //지정한 모듈의 첫 번째 입력 채널 번호를 확인한다.(입력 모듈, 입력/출력 통합 모듈용)
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetChannelNoAdcOfModuleNo(int lModuleNo, ref int lpChannelNo);

                //지정한 모듈의 첫 번째 출력 채널 번호를 확인한다.(출력 모듈, 입력/출력 통합 모듈용)
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetChannelNoDacOfModuleNo(int lModuleNo, ref int lpChannelNo);

                //지정한 모듈 번호로 베이스 보드 번호, 모듈 위치, 모듈 ID를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetModule(int lModuleNo, ref int lpBoardNo, ref int lpModulePos, ref uint upModuleID);

                // 해당 모듈이 제어가 가능한 상태인지 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaInfoGetModuleStatus(int lModuleNo);

                //========== 입력 모듈 정보 검색 함수 ================================================================
                //지정한 입력 채널 번호로 모듈 번호를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiInfoGetModuleNoOfChannelNo(int lChannelNo, ref int lpModuleNo);

                //아날로그 입력 모듈의 전체 채널 개수를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiInfoGetChannelCount(ref int lpChannelCount);

                //========== 입력 모듈 인터럽트/채널 이벤트 설정 및 확인 함수 ====================================
                //지정한 채널에 이벤트 메시지를 받아오기 위하여 윈도우 메시지, 콜백 함수 또는 이벤트 방식을 사용한다. H/W 타이머(Timer Trigger Mode, External Trigger Mode)를 이용, 연속적 데이터 수집 동작시(AxaStartMultiChannelAdc 참조)에 사용한다.
                // 지정한 모듈에 인터럽트 메시지를 받아오기 위하여 윈도우 메시지, 콜백 함수 또는 이벤트 방식을 사용
                //========= 인터럽트 관련 함수 ======================================================================================
                // 콜백 함수 방식은 이벤트 발생 시점에 즉시 콜백 함수가 호출 됨으로 가장 빠르게 이벤트를 통지받을 수 있는 장점이 있으나
                // 콜백 함수가 완전히 종료 될 때까지 메인 프로세스가 정체되어 있게 된다.
                // 즉, 콜백 함수 내에 부하가 걸리는 작업이 있을 경우에는 사용에 주의를 요한다. 
                // 이벤트 방식은 쓰레드등을 이용하여 인터럽트 발생여부를 지속적으로 감시하고 있다가 인터럽트가 발생하면 
                // 처리해주는 방법으로, 쓰레드 등으로 인해 시스템 자원을 점유하고 있는 단점이 있지만
                // 가장 빠르게 인터럽트를 검출하고 처리해줄 수 있는 장점이 있다.
                // 일반적으로는 많이 쓰이지 않지만, 인터럽트의 빠른처리가 주요 관심사인 경우에 사용된다. 
                // 이벤트 방식은 이벤트의 발생 여부를 감시하는 특정 쓰레드를 사용하여 메인 프로세스와 별개로 동작되므로
                // MultiProcessor 시스템등에서 자원을 가장 효율적으로 사용할 수 있게 되어 특히 권장하는 방식이다.
                // 인터럽트 메시지를 받아오기 위하여 윈도우 메시지 또는 콜백 함수를 사용한다.
                // (메시지 핸들, 메시지 ID, 콜백함수, 인터럽트 이벤트)
                //    hWnd            : 윈도우 핸들, 윈도우 메세지를 받을때 사용. 사용하지 않으면 NULL을 입력.
                //    uMessage        : 윈도우 핸들의 메세지, 사용하지 않거나 디폴트값을 사용하려면 0을 입력.
                //    proc            : 인터럽트 발생시 호출될 함수의 포인터, 사용하지 않으면 NULL을 입력.
                //    pEvent          : 이벤트 방법사용시 이벤트 핸들
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventSetChannel(int lModuleNo, IntPtr hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);

                //지정한 입력 채널에 이벤트 사용 유무를 설정한다.
                //======================================================
                // uUse        : DISABLE(0)    // 이벤트 해제
                //             : ENABLE(1)     // 이벤트 설정
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventSetChannelEnable(int lChannelNo, uint uUse);

                //지정한 입력 채널의 이벤트 사용 유무를 확인한다.
                //======================================================
                // *upUse      : DISABLE(0)    // 이벤트 해제
                //             : ENABLE(1)     // 이벤트 설정
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventGetChannelEnable(int lChannelNo, ref uint upUse);

                //지정한 여러 입력 채널에 이벤트 사용 유무를 설정한다.
                //======================================================
                // lSize       : 사용 할 입력 채널의 갯수
                // lpChannelNo : 사용할 채널 번호의 배열
                // uUse        : DISABLE(0)    // 이벤트 해제
                //             : ENABLE(1)     // 이벤트 설정
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventSetMultiChannelEnable(int lSize, int[] lpChannelNo, uint uUse);

                //지정한 입력 채널에 이벤트 종류를 설정한다.
                //======================================================
                // uMask       : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
                //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
                //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
                //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventSetChannelMask(int lChannelNo, uint uMask);

                //지정한 입력 채널에 이벤트 종류를 확인한다.
                //======================================================
                // *upMask     : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
                //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
                //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
                //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventGetChannelMask(int lChannelNo, ref uint upMask);

                //지정한 여러 입력 채널에 이벤트 종류를 설정한다.
                //======================================================
                // lSize       : 사용 할 입력 채널의 갯수
                // lpChannelNo : 사용할 채널 번호의 배열
                // uMask       : DATA_EMPTY(1) --> 버퍼에 데이터가 없을 때
                //             : DATA_MANY(2)  --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
                //             : DATA_SMALL(3) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
                //             : DATA_FULL(4)  --> 버퍼에 데이터가 꽉 찼을 때
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventSetMultiChannelMask(int lSize, int[] lpChannelNo, uint uMask);

                //이벤트 발생 위치를 확인한다.
                //======================================================
                // *upMode     : AIO_EVENT_DATA_UPPER(1) --> 버퍼에 데이터가 상한 설정 값보다 많아질 때
                //             : AIO_EVENT_DATA_LOWER(2) --> 버퍼에 데이터가 하한 설정 값보다 적어질 때
                //             : AIO_EVENT_DATA_FULL(3)  --> 버퍼에 데이터가 꽉 찼을 때
                //             : AIO_EVENT_DATA_EMPTY(4) --> 버퍼에 데이터가 없을 때
                //======================================================
                [DllImport("AXL.dll")]
                public static extern uint AxaiEventRead(ref int lpChannelNo, ref uint upMode);

                //지정한 모듈의 인터럽트 마스크를 설정한다. 이 함수는 연속적 신호감시를 할 경우에 하드웨어(모듈)의 FIFO 에서 사용자가 
                //지정한 크기의 버퍼로 내부 인터럽트를 통한 데이터 이동 시점을 지정하기 위해 사용된다. (SIO-AI4RB는 지원하지 않는다.)
                //==================================================================================================//
                // uMask       : SCAN_END(1)       --> 셋팅된 채널 모두  ADC 변환이 한번 이루어 질 때 마다 인터럽트가 발생
                //             : FIFO_HALF_FULL(2) --> 모듈내의 FIFO가 HALF이상 찼을 경우 내부 인터럽트 발생
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiInterruptSetModuleMask(int lModuleNo, uint uMask);

                //지정한 모듈의 인터럽트 마스크를 확인한다.
                //==================================================================================================//
                // *upMask     : SCAN_END(1)       --> 셋팅된 채널 모두  ADC 변환이 한번 이루어 질 때 마다 인터럽트가 발생
                //             : FIFO_HALF_FULL(2) --> 모듈내의 FIFO가 HALF이상 찼을 경우 내부 인터럽트 발생
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiInterruptGetModuleMask(int lModuleNo, ref uint upMask);

                //========== 입력 모듈 파라미터 설정 및 확인 함수 ========================================================================
                //지정한 입력 채널에 입력 전압 범위를 설정한다.
                //==================================================================================================//
                // AI4RB
                // dMinVolt    : -10V/-5V로 설정 가능
                // dMaxVolt    : 10V/5V/로 설정 가능
                //
                // AI16Hx
                // dMinVolt    : -10V 고정
                // dMaxVolt    : 10V 고정
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiSetRange(int lChannelNo, double dMinVolt, double dMaxVolt);

                //지정한 입력 채널의 입력 전압 범위를 확인한다.
                //==================================================================================================//
                // AI4RB
                // *dpMinVolt  : -10V/-5V로 설정 가능
                // *dpMaxVolt  : 10V/5V/로 설정 가능
                //
                // AI16Hx
                // *dpMaxVolt  : -10V 고정
                // *dpMaxVolt  : 10V 고정
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiGetRange(int lChannelNo, ref double dpMinVolt, ref double dpMaxVolt);

                //지정한 여러 입력 모듈에 허용 입력 전압 범위를 설정한다.
                //==================================================================================================//
                // lModuleNo   : 사용할 입력 모듈 번호
                //
                // RTEX AI16F
                // Mode -5~+5  : dMinVolt = -5, dMaxVolt = +5
                // Mode -10~+10: dMinVolt = -10, dMaxVolt = +10
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiSetRangeModule(int lModuleNo, double dMinVolt, double dMaxVolt);

                //지정한 여러 입력 모듈에 허용 입력 전압 범위를 확인한다.
                //==================================================================================================//
                // lModuleNo   : 사용할 입력 모듈 번호
                //
                // RTEX AI16F
                // *dMinVolt   : -5V, -10V
                // *dMaxVolt   : +5V, +10V
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiGetRangeModule(int lModuleNo, ref double dMinVolt, ref double dMaxVolt);

                //지정한 여러 입력 채널에 허용 입력 전압 범위를 설정한다.
                //==================================================================================================//
                // lSize        : 사용할 입력 채널의 개수
                // *lpChannelNo : 사용할 채널 번호의 배열
                //
                // AI4RB
                // dMinVolt    : -10V/-5V로 설정 가능
                // dMaxVolt    : 10V/5V/로 설정 가능
                //
                // AI16Hx
                // dMinVolt    : -10V
                // dMaxVolt    : 10V
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiSetMultiRange(int lSize, int[] lpChannelNo, double dMinVolt, double dMaxVolt);

                //지정한 입력 모듈에 트리거 모드를 설정한다.
                //==================================================================================================//
                // uTriggerMode : NORMAL_MODE(1)   --> 사용자가 원하는 시점에 A/D변환하는 Software Trigger 방식 
                //              : TIMER_MODE(2)    --> H/W의 내부 클럭을 이용해서 A/D변환하는 Trigger 방식
                //              : EXTERNAL_MODE(3) --> 외부 입력단자의 클럭을 이용해서 A/D변환하는 Trigger 방식
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiSetTriggerMode(int lModuleNo, uint uTriggerMode);

                //지정한 모듈에 트리거 모드를 확인한다.
                //==================================================================================================//
                // *upTriggerMode : NORMAL_MODE(1)   --> 사용자가 원하는 시점에 A/D변환하는 Software Trigger 방식 
                //                : TIMER_MODE(2)    --> H/W의 내부 클럭을 이용해서 A/D변환하는 Trigger 방식
                //                : EXTERNAL_MODE(3) --> 외부 입력단자의 클럭을 이용해서 A/D변환하는 Trigger 방식
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiGetTriggerMode(int lModuleNo, ref uint upTriggerMode);

                //지정한 입력모듈의 Offset을 mVolt 단위(mV)로 설정한다. 최대 -100~100mVolt
                //==================================================================================================//
                // dMiliVolt    : -100 ~ 100 
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiSetModuleOffsetValue(int lModuleNo, double dMiliVolt);

                //지정한 입력모듈의 Offset 값을 확인한다. mVolt 단위(mV)
                //==================================================================================================//
                // *dpMiliVolt  : -100 ~ 100 
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiGetModuleOffsetValue(int lModuleNo, ref double dpMiliVolt);

                //========== 입력 모듈 A/D 변환 함수
                //==Software Trigger Mode 함수
                //사용자가 지정한 입력 채널에 아날로그 입력 값을 A/D변환한 후 전압 값으로 반환한다.이 함수를 사용하기 전에 AxaSetTriggerModeAdc 함수를 사용하여 Normal Trigger Mode로 설정되어 있어야 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiSwReadVoltage(int lChannelNo, ref double dpVolt);

                //지정한 입력 채널에 아날로그 입력 값을 Digit 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiSwReadDigit(int lChannelNo, ref uint upDigit);

                //지정한 여러 입력 채널에 아날로그 입력 값을 전압 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiSwReadMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);

                //지정한 여러 입력 채널에 아날로그 입력 값을 Digit 값으로 반환한다. Normal Trigger Mode로 설정되어 있어야 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiSwReadMultiDigit(int lSize, int[] lpChannelNo, uint[] upDigit);

                //지정한 여러 입력 채널에 Immediate모드를 사용하기 위해 설정 값을 설정한다. 이 함수를 사용하기 전에 AxaSetTriggerModeAdc 함수를 사용하여 Timer Trigger Mode로 설정되어 있어야 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetMultiAccess(int lSize, int[] lpChannelNo, int[] lpWordSize);

                //지정한 개수만큼 A/D변환 후 전압 값을 반환한다. 이 함수를 사용하기 전에 AxaiHwSetMultiAccess함수를 이용 설정값을 지정해야 하며 , AxaSetTriggerModeAdc 함수를 사용하여 Timer Trigger Mode로 설정되어 있어야 한다.
                // [DllImport("AXL.dll")] public static extern uint AxaiHwStartMultiAccess(ref double[] dpBuffer);
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwStartMultiAccess(double[,] dpBuffer);

                //지정한 모듈에 샘플링 간격을 주파수 단위로 설정한다.
                //==================================================================================================//
                // dSampleFreq    : 10 ~ 100000 
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetSampleFreq(int lModuleNo, double dSampleFreq);

                //지정한 모듈에 샘플링 간격을 주파수 단위로 설정된 값을 확인한다.
                //==================================================================================================//
                // *dpSampleFreq  : 10 ~ 100000 
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwGetSampleFreq(int lModuleNo, ref double dpSampleFreq);

                //지정한 모듈에 샘플링 간격을 시간 단위(uSec)로 설정한다.
                //==================================================================================================//
                // dSamplePeriod  : 100000 ~ 1000000000
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetSamplePeriod(int lModuleNo, double dSamplePeriod);

                //지정한 모듈에 샘플링 간격을 시간 단위(uSec)로 설정된 값을 확인한다.
                //==================================================================================================//
                // *dpSamplePeriod: 100000 ~ 1000000000
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwGetSamplePeriod(int lModuleNo, ref double dpSamplePeriod);

                //지정한 입력 채널에 버퍼가 Full로 찼을 때 관리 방식을 설정한다.
                //==================================================================================================//
                // uFullMode      : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
                //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetBufferOverflowMode(int lChannelNo, uint uFullMode);

                //지정한 입력 채널이 버퍼가 Full로 찼을 때 관리 방식을 확인한다.
                //==================================================================================================//
                // *upFullMode    : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
                //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwGetBufferOverflowMode(int lChannelNo, ref uint upFullMode);

                //지정한 여러 입력 채널에 버퍼가 Full로 찼을 때 관리 방식을 설정한다.
                //==================================================================================================//
                // uFullMode      : NEW_DATA_KEEP(0) --> 새로운 데이터 유지
                //                : CURR_DATA_KEEP(1) --> 이전 데이터 유지
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetMultiBufferOverflowMode(int lSize, int[] lpChannelNo, uint uFullMode);

                //지정한 입력 채널에 버퍼의 상한 값과 하한 값을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetLimit(int lChannelNo, int lLowLimit, int lUpLimit);

                //지정한 입력 채널에 버퍼의 상한 값과 하한 값을 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwGetLimit(int lChannelNo, ref int lpLowLimit, ref int lpUpLimit);

                //지정한 여러 입력 채널에 버퍼의 상한 값과 하한 값을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwSetMultiLimit(int lSize, int[] lpChannelNo, int lLowLimit, int lUpLimit);

                //지정한 여러 입력 채널에 H/W타이머를 이용한 A/D변환을 시작한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwStartMultiChannel(int lSize, int[] lpChannelNo, int lBuffSize);

                //지정한 여러 입력 채널에 A/D변환을 시작 후 지정한 개수만큼 필터 처리해서 전압으로 반환한다.
                //==================================================================================================//
                // lSize          : 사용할 입력 채널의 개수
                // *lpChannelNo   : 사용할 채널 번호의 배열
                // lFilterCount   : Filtering할 데이터의 개수
                // lBuffSize      : 각 채널에 할당되는 버퍼의 개수
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwStartMultiFilter(int lSize, int[] lpChannelNo, int lFilterCount, int lBuffSize);

                //H/W타이머를 이용한 연속 신호 A/D변환을 중지한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwStopMultiChannel(int lModuleNo);

                //지정한 입력 채널의 메모리 버퍼에 데이터가 몇 개인지 검사한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwReadDataLength(int lChannelNo, ref int lpDataLength);

                //지정한 입력 채널에 H/W타이머를 이용하여 A/D변환된 값을 전압 값으로 읽는다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwReadSampleVoltage(int lChannelNo, ref int lpSize, ref double dpVolt);

                //지정한 입력 채널에 H/W타이머를 이용하여 A/D변환된 값을 Digit 값으로 읽는다.
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwReadSampleDigit(int lChannelNo, ref int lpSize, ref uint upDigit);

                //========== 입력 모듈 버퍼 상태 체크 함수 ===============================================================================
                //지정한 입력 채널의 메모리 버퍼에 데이터가 없는 지 검사한다.
                //==================================================================================================//
                // *upEmpty       : FALSE(0) --> 메모리 버퍼에 데이터가 있을 경우
                //                : TRUE(1)  --> 메모리 버퍼에 데이터가 없을 경우
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwIsBufferEmpty(int lChannelNo, ref uint upEmpty);

                //지정한 입력 채널의 메모리 버퍼에 설정되어 있는 상한 값보다 데이터가 많은 지 검사한다
                //==================================================================================================//
                // *upUpper       : FALSE(0) --> 메모리 버퍼에 데이터가 상한 값보다 적을 경우
                //                : TRUE(1)  --> 메모리 버퍼에 데이터가 상한 값보다 많을 경우
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwIsBufferUpper(int lChannelNo, ref uint upUpper);

                //지정한 입력 채널의 메모리 버퍼에 설정되어 있는 하한 값보다 데이터가 적은 지 검사한다.
                //==================================================================================================//
                // *upLower       : FALSE(0) --> 메모리 버퍼에 데이터가 하한 값보다 많을 경우
                //                : TRUE(1)  --> 메모리 버퍼에 데이터가 하한 값보다 적을 경우
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaiHwIsBufferLower(int lChannelNo, ref uint upLower);

                //==External Trigger Mode 함수
                //지정한 입력모듈의 선택된 채널들의 외부 트리거 모드를 시작한다.
                //==================================================================================================//
                // lSize          : 지정한 입력 모듈에서 외부트리거를 사용 할 채널갯수
                // *lpChannelPos  : 지정한 입력 모듈에서 외부트리거를 사용 할 채널의 Index
                [DllImport("AXL.dll")]
                public static extern uint AxaiExternalStartADC(int lModuleNo, int lSize, ref int lpChannelPos);

                //지정한 입력모듈의 외부트리거 모드를 정지한다.  
                [DllImport("AXL.dll")]
                public static extern uint AxaiExternalStopADC(int lModuleNo);

                //지정한 입력모듈의 Fifo상태를 반환한다.
                //==================================================================================================//
                // *dwpStatus     : FIFO_DATA_EXIST(0)
                //                : FIFO_DATA_EMPTY(1)
                //                : FIFO_DATA_HALF(2)
                //                : FIFO_DATA_FULL(6)
                //==================================================================================================//    
                [DllImport("AXL.dll")]
                public static extern uint AxaiExternalReadFifoStatus(int lModuleNo, ref uint upStatus);

                //지정한 입력모듈의 외부신호에 의해 변환된 A/D값을 읽어옴.
                // lSize          : 지정한 입력 모듈에서 변환된 A/D값을 읽어올 채널의 갯수(AxaiExternalStartADC에 사용한 채널갯수와 동일 해야됨)
                // *lpChannelPos  : 지정한 입력 모듈에서 변환된 A/D값을 읽어올 채널의 Index(AxaiExternalStartADC에 사용한 채널의 Index와 동일 해야됨)
                // lDataSize      : 외부트리거에 의해 A/D변환된 값을 한번에 읽어 올 최대 데이타의 갯수
                // lBuffSize      : 외부에서(사용자 Program) 할당한 Data Buffer의 Size
                // lStartDataPos  : 외부에서(사용자 Program) 할당한 Data Buffer에 저장 시작 할 위치 
                // *dpVolt[]      : A/D변환된 값을 할당 받을 2차원 배열 포인트(dpVlot[Channel][Count])
                // *lpRetDataSize : A/D변환된 값이 Data Buffer에 실제 할당된 갯수
                // *dwpStatus     : A/D변환된 값을 Fifo(H/W Buffer)로 부터 읽을 때 Fifo상태를 반환함.
                [DllImport("AXL.dll")]
                public static extern uint AxaiExternalReadVoltage(int lModuleNo, int lSize, ref int lpChannelPos, int lDataSize, int lBuffSize, int lStartDataPos, double[,] dpVolt, ref int lpRetDataSize, ref uint upStatus);

                //========== 출력 모듈 정보 검색 함수 ====================================================================================
                //지정한 출력 채널 번호로 모듈 번호를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoInfoGetModuleNoOfChannelNo(int lChannelNo, ref int lpModuleNo);

                //아날로그 출력 모듈의 전체 채널 개수를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoInfoGetChannelCount(ref int lpChannelCount);

                //========== 출력 모듈 설정 및 확인 함수 =================================================================================
                //지정한 출력 채널에 출력 전압 범위를 설정한다
                //==================================================================================================//
                // AXT_SIO_RAO4RB
                // dMinVolt    : -10V
                // dMaxVolt    : 10V
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaoSetRange(int lChannelNo, double dMinVolt, double dMaxVolt);

                //지정한 출력 채널의 출력 전압 범위를 확인한다.
                //==================================================================================================//
                // AXT_SIO_RAO4RB
                // *dpMinVolt    : -10V
                // *dpMaxVolt    : 10V
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaoGetRange(int lChannelNo, ref double dpMinVolt, ref double dpMaxVolt);

                //지정한 여러 출력 채널에 출력 전압 범위를 설정한다.
                //==================================================================================================//
                // AO4R, AO2Hx
                // dMinVolt    : -10V
                // dMaxVolt    : 10V
                //==================================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxaoSetMultiRange(int lSize, int[] lpChannelNo, double dMinVolt, double dMaxVolt);

                //지정한 출력 채널에 입력된 전압이 출력 된다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoWriteVoltage(int lChannelNo, double dVolt);

                //지정한 여러 출력 채널에 입력된 전압이 출력 된다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoWriteMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);

                //지정한 출력 채널에 출력되는 전압 값을 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoReadVoltage(int lChannelNo, ref double dpVolt);

                //지정한 여러 출력 채널에 출력되는 전압 값을 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxaoReadMultiVoltage(int lSize, int[] lpChannelNo, double[] dpVolt);
            }
        }

        protected struct ChannelInfo
        {
            public ushort ChannelNo { get; set; }
            public byte[] IO { get; set; }
        };

        private Dictionary<int, ChannelInfo> _inputChannelInfoList = new Dictionary<int, ChannelInfo>();
        protected Dictionary<int, ChannelInfo> InputChannelInfoList { get { return _inputChannelInfoList; } }

        private Dictionary<int, ChannelInfo> _outputChannelInfoList = new Dictionary<int, ChannelInfo>();
        protected Dictionary<int, ChannelInfo> OutputChannelInfoList { get { return _outputChannelInfoList; } }
        
        public ushort BitRate { get; set; }
        public ushort InputChannelCount { get; set; }
        public ushort OutputChannelCount { get; set; }
        public double ReadRangeMin { get; set; }
        public double ReadRangeMax { get; set; }
        public double WriteRangeMin { get; set; }
        public double WriteRangeMax { get; set; }

        public override void Open()
        {
            uint result = 0;
            if (AjinLibrary.CAXL.AxlIsOpened() != (int)AjinLibrary.AXT_BOOLEAN.TRUE)
            {
                result = AjinLibrary.CAXL.AxlOpenNoReset(0);

                if (result != (uint)AjinLibrary.AXT_FUNC_RESULT.AXT_RT_SUCCESS)
                    throw new Exception(this.Name + " is initializing fail");                
            }

            AjinLibrary.CAXA.AxaiSetRange(0, ReadRangeMin, ReadRangeMax);
            AjinLibrary.CAXA.AxaoSetRange(0, WriteRangeMin, WriteRangeMax);
        }

        public override void Close()
        {
            AjinLibrary.CAXL.AxlClose();
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            SetChannelInfo();
        }

        public override bool GetInputIOValue(int index)
        {
            try
            {
                int channelID = index / 10000 - 1;
                int byteIndex = (index - (channelID + 1) * 10000) / 10;
                int bitIndex = index % 10;
                return Utility.FAUtility.CheckBit(InputChannelInfoList[channelID].IO[byteIndex], bitIndex);
            }
            catch
            {
                return false;
            }
        }

        public override void SetInputIOValue(int index, bool value) //Simulation에서만 사용
        {
            try
            {
                int channelID = (ushort)(index / 10000) - 1;
                int byteIndex = (index - (channelID + 1) * 10000) / 10;
                int bitIndex = index % 10;
                OutputChannelInfoList[channelID].IO[byteIndex] =
                    Utility.FAUtility.SetBit(InputChannelInfoList[channelID].IO[byteIndex], bitIndex, value);
            }
            catch
            {
            }
        }

        public override bool GetOutputIOValue(int index)
        {
            try
            {
                int channelID = index / 10000 - 1;
                int byteIndex = (index - (channelID + 1) * 10000) / 10;
                int bitIndex = index % 10;
                if (OutputChannelInfoList.ContainsKey(channelID) == false) return false;
                if (OutputChannelInfoList[channelID].IO.Length <= byteIndex) return false;

                return Utility.FAUtility.CheckBit(OutputChannelInfoList[channelID].IO[byteIndex], bitIndex);
            }
            catch
            {
                return false;
            }
        }

        public override void SetOutputIOValue(int index, bool value)
        {
            try
            {
                int channelID = (ushort)(index / 10000) - 1;
                int byteIndex = (index - (channelID + 1) * 10000) / 10;
                int bitIndex = index % 10;
                OutputChannelInfoList[channelID].IO[byteIndex] =
                    Utility.FAUtility.SetBit(OutputChannelInfoList[channelID].IO[byteIndex], bitIndex, value);
            }
            catch
            {
            }
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
            int channelID = index / 10000 - 1;
            int byteIndex = (index - (channelID + 1) * 10000) / 10;
            if (InputChannelInfoList.ContainsKey(channelID) == false) return;
            if (InputChannelInfoList[channelID].IO.Length <= byteIndex) return;

            for (int i = 0; i < bytes.Length; i++)
            {
                int currentIndex = byteIndex + i;
                if (InputChannelInfoList[channelID].IO.Length <= currentIndex) break;
                bytes[i] = InputChannelInfoList[channelID].IO[currentIndex];
            }
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
            try
            {
                int channelID = (ushort)(index / 10000) - 1;
                int byteIndex = (index - (channelID + 1) * 10000) / 10;

                for (int i = 0; i < bytes.Length; i++)
                {
                    int currentIndex = byteIndex + i;
                    if (OutputChannelInfoList[channelID].IO.Length <= currentIndex) break;
                    OutputChannelInfoList[channelID].IO[currentIndex] = bytes[i];
                }
            }
            catch
            {
            }
        }

        public override void ReadWrite()
        {
            foreach (KeyValuePair<int, ChannelInfo> channelInfo in InputChannelInfoList)
            {
                double volt = 0;
                AjinLibrary.CAXA.AxaiSwReadVoltage(channelInfo.Value.ChannelNo, ref volt);
                var bytes = BitConverter.GetBytes(volt);
                for (int i = 0; i < channelInfo.Value.IO.Length; i++)
                {
                    channelInfo.Value.IO[i] = bytes[i];
                }
            }

            foreach (KeyValuePair<int, ChannelInfo> channelInfo in OutputChannelInfoList)
            {
                double outputValue = BitConverter.ToDouble(channelInfo.Value.IO, 0);                
                AjinLibrary.CAXA.AxaoWriteVoltage(channelInfo.Value.ChannelNo, outputValue);
            }
        }

        private void SetChannelInfo()
        {            
            for (ushort i = 0; i < InputChannelCount; i++)
            {
                ChannelInfo channelInfo = new ChannelInfo();
                channelInfo.ChannelNo = i;
                channelInfo.IO = new byte[8];
                InputChannelInfoList.Add(i, channelInfo);
            }

            for (ushort i = 0; i < OutputChannelCount; i++)
            {
                ChannelInfo channelInfo = new ChannelInfo();
                channelInfo.ChannelNo = i;
                channelInfo.IO = new byte[sizeof(double)];
                OutputChannelInfoList.Add(i, channelInfo);
            }
        }
    }
}
