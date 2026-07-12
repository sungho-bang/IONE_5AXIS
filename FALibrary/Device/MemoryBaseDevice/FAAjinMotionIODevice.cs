using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Xml.Linq;

namespace FALibrary.Device.MemoryBaseDevice
{
    public class FAAjinMotionIODevice : FAMemoryBaseDevice
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

            public class CAXM
            {

                //========== 보드 및 모듈 확인함수(Info) - Information =================================================================================

                // 해당 축의 보드번호, 모듈 위치, 모듈 아이디를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInfoGetAxis(int nAxisNo, ref int npBoardNo, ref int npModulePos, ref uint upModuleID);
                // 모션 모듈이 존재하는지 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInfoIsMotionModule(ref uint upStatus);
                // 해당 축이 유효한지 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInfoIsInvalidAxisNo(int nAxisNo);
                // CAMC-IP, CAMC-QI 축 개수, 시스템에 장착된 유효한 모션 축수를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInfoGetAxisCount(ref int npAxisCount);
                // 해당 보드/모듈의 첫번째 축번호를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInfoGetFirstAxisNo(int nBoardNo, int nModulePos, ref int npAxisNo);

                //========= 가상 축 함수 ============================================================================================    
                // 초기 상태에서 AXM 모든 함수의 축번호 설정은 0 ~ (실제 시스템에 장착된 축수 - 1) 범위에서 유효하지만
                // 이 함수를 사용하여 실제 장착된 축번호 대신 임의의 축번호로 바꿀 수 있다.
                // 이 함수는 제어 시스템의 H/W 변경사항 발생시 기존 프로그램에 할당된 축번호를 그대로 유지하고 실제 제어 축의 
                // 물리적인 위치를 변경하여 사용을 위해 만들어진 함수이다.
                // 주의사항 : 여러 개의 실제 축번호에 대하여 같은 번호로 가상 축을 중복해서 맵핑할 경우 
                //            실제 축번호가 낮은 축만 가상 축번호로 제어 할 수 있으며, 
                //            나머지 같은 가상축 번호로 맵핑된 축은 제어가 불가능한 경우가 발생 할 수 있다.

                // 가상축을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmVirtualSetAxisNoMap(int nRealAxisNo, int nVirtualAxisNo);
                // 설정한 가상채널(축) 번호를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmVirtualGetAxisNoMap(int nRealAxisNo, ref int npVirtualAxisNo);
                // 멀티 가상축을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmVirtualSetMultiAxisNoMap(int nSize, ref int npRealAxesNo, ref int npVirtualAxesNo);
                // 설정한 멀티 가상채널(축) 번호를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmVirtualGetMultiAxisNoMap(int nSize, ref int npRealAxesNo, ref int npVirtualAxesNo);
                // 가상축 설정을 해지한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmVirtualResetAxisMap();

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
                //    hWnd    : 윈도우 핸들, 윈도우 메세지를 받을때 사용. 사용하지 않으면 NULL을 입력.
                //    wMsg    : 윈도우 핸들의 메세지, 사용하지 않거나 디폴트값을 사용하려면 0을 입력.
                //    proc    : 인터럽트 발생시 호출될 함수의 포인터, 사용하지 않으면 NULL을 입력.
                //    pEvent  : 이벤트 방법사용시 이벤트 핸들
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptSetAxis(int nAxisNo, uint hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);

                // 설정 축의 인터럽트 사용 여부를 설정한다
                // 해당 축에 인터럽트 설정 / 확인
                // uUse : 사용 유무 => DISABLE(0), ENABLE(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptSetAxisEnable(int nAxisNo, uint uUse);
                // 설정 축의 인터럽트 사용 여부를 반환한다
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptGetAxisEnable(int nAxisNo, ref uint upUse);

                //인터럽트를 이벤트 방식으로 사용할 경우 해당 인터럽트 정보 읽는다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptRead(ref int npAxisNo, ref uint upFlag);

                // 해당 축의 인터럽트 플래그 값을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptReadAxisFlag(int nAxisNo, int nBank, ref uint upFlag);

                // 지정 축의 사용자가 설정한 인터럽트 발생 여부를 설정한다.
                // lBank         : 인터럽트 뱅크 번호 (0 - 1) 설정가능.
                // uInterruptNum : 인터럽트 번호 설정 비트번호로 설정 hex값 혹은 define된값을 설정
                // AXHS.h파일에 IP, QI INTERRUPT_BANK1, 2 DEF를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptSetUserEnable(int nAxisNo, int lBank, uint uInterruptNum);

                // 지정 축의 사용자가 설정한 인터럽트 발생 여부를 확인한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmInterruptGetUserEnable(int nAxisNo, int lBank, ref uint upInterruptNum);


                //======== 모션 파라메타 설정 ===========================================================================================================================================================
                // AxmMotLoadParaAll로 파일을 Load 시키지 않으면 초기 파라메타 설정시 기본 파라메타 설정. 
                // 현재 PC에 사용되는 모든축에 똑같이 적용된다. 기본파라메타는 아래와 같다. 
                // 00:AXIS_NO.             =0       01:PULSE_OUT_METHOD.    =4      02:ENC_INPUT_METHOD.    =3     03:INPOSITION.          =2
                // 04:ALARM.               =0       05:NEG_END_LIMIT.       =0      06:POS_END_LIMIT.       =0     07:MIN_VELOCITY.        =1
                // 08:MAX_VELOCITY.        =700000  09:HOME_SIGNAL.         =4      10:HOME_LEVEL.          =1     11:HOME_DIR.            =-1
                // 12:ZPHASE_LEVEL.        =1       13:ZPHASE_USE.          =0      14:STOP_SIGNAL_MODE.    =0     15:STOP_SIGNAL_LEVEL.   =0
                // 16:HOME_FIRST_VELOCITY. =10000   17:HOME_SECOND_VELOCITY.=10000  18:HOME_THIRD_VELOCITY. =2000  19:HOME_LAST_VELOCITY.  =100
                // 20:HOME_FIRST_ACCEL.    =40000   21:HOME_SECOND_ACCEL.   =40000  22:HOME_END_CLEAR_TIME. =1000  23:HOME_END_OFFSET.     =0
                // 24:NEG_SOFT_LIMIT.      =0.000   25:POS_SOFT_LIMIT.      =0      26:MOVE_PULSE.          =1     27:MOVE_UNIT.           =1
                // 28:INIT_POSITION.       =1000    29:INIT_VELOCITY.       =200    30:INIT_ACCEL.          =400   31:INIT_DECEL.          =400
                // 32:INIT_ABSRELMODE.     =0       33:INIT_PROFILEMODE.    =4

                // 00=[AXIS_NO             ]: 축 (0축 부터 시작함)
                // 01=[PULSE_OUT_METHOD    ]: Pulse out method TwocwccwHigh = 6
                // 02=[ENC_INPUT_METHOD    ]: disable = 0   1체배 = 1  2체배 = 2  4체배 = 3, 결선 관련방향 교체시(-).1체배 = 11  2체배 = 12  4체배 = 13
                // 03=[INPOSITION          ], 04=[ALARM     ], 05,06 =[END_LIMIT   ]  : 0 = A접점 1= B접점 2 = 사용안함. 3 = 기존상태 유지
                // 07=[MIN_VELOCITY        ]: 시작 속도(START VELOCITY)
                // 08=[MAX_VELOCITY        ]: 드라이버가 지령을 받아들일수 있는 지령 속도. 보통 일반 Servo는 700k
                // Ex> screw : 20mm pitch drive: 10000 pulse 모터: 400w
                // 09=[HOME_SIGNAL         ]: 4 - Home in0 , 0 :PosEndLimit , 1 : NegEndLimit // _HOME_SIGNAL참조.
                // 10=[HOME_LEVEL          ]: 0 = A접점 1= B접점 2 = 사용안함. 3 = 기존상태 유지
                // 11=[HOME_DIR            ]: 홈 방향(HOME DIRECTION) 1:+방향, 0:-방향
                // 12=[ZPHASE_LEVEL        ]: 0 = A접점 1= B접점 2 = 사용안함. 3 = 기존상태 유지
                // 13=[ZPHASE_USE          ]: Z상사용여부. 0: 사용안함 , 1: +방향, 2: -방향 
                // 14=[STOP_SIGNAL_MODE    ]: ESTOP, SSTOP 사용시 모드 0:감속정지, 1:급정지 
                // 15=[STOP_SIGNAL_LEVEL   ]: ESTOP, SSTOP 사용 레벨.  0 = A접점 1= B접점 2 = 사용안함. 3 = 기존상태 유지 
                // 16=[HOME_FIRST_VELOCITY ]: 1차구동속도 
                // 17=[HOME_SECOND_VELOCITY]: 검출후속도 
                // 18=[HOME_THIRD_VELOCITY ]: 마지막 속도 
                // 19=[HOME_LAST_VELOCITY  ]: index검색및 정밀하게 검색하기위한 속도. 
                // 20=[HOME_FIRST_ACCEL    ]: 1차 가속도 , 21=[HOME_SECOND_ACCEL   ] : 2차 가속도 
                // 22=[HOME_END_CLEAR_TIME ]: 원점 검색 Enc 값 Set하기 위한 대기시간,  23=[HOME_END_OFFSET] : 원점검출후 Offset만큼 이동.
                // 24=[NEG_SOFT_LIMIT      ]: - SoftWare Limit 같게 설정하면 사용안함, 25=[POS_SOFT_LIMIT ]: + SoftWare Limit 같게 설정하면 사용안함.
                // 26=[MOVE_PULSE          ]: 드라이버의 1회전당 펄스량              , 27=[MOVE_UNIT  ]: 드라이버 1회전당 이동량 즉:스크류 Pitch
                // 28=[INIT_POSITION       ]: 에이젼트 사용시 초기위치  , 사용자가 임의로 사용가능
                // 29=[INIT_VELOCITY       ]: 에이젼트 사용시 초기속도  , 사용자가 임의로 사용가능
                // 30=[INIT_ACCEL          ]: 에이젼트 사용시 초기가속도, 사용자가 임의로 사용가능
                // 31=[INIT_DECEL          ]: 에이젼트 사용시 초기감속도, 사용자가 임의로 사용가능
                // 32=[INIT_ABSRELMODE     ]: 절대(0)/상대(1) 위치 설정
                // 33=[INIT_PROFILEMODE    ]: 프로파일모드(0 - 4) 까지 설정
                //                            '0': 대칭 Trapezode, '1': 비대칭 Trapezode, '2': 대칭 Quasi-S Curve, '3':대칭 S Curve, '4':비대칭 S Curve

                // AxmMotSaveParaAll로 저장 되어진 .mot파일을 불러온다. 해당 파일은 사용자가 Edit 하여 사용 가능하다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotLoadParaAll(string szFilePath);
                // 모든축에 대한 모든 파라메타를 축별로 저장한다. .mot파일로 저장한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSaveParaAll(string szFilePath);

                // 파라메타 28 - 31번까지 사용자가 프로그램내에서  이 함수를 이용해 설정 한다
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetParaLoad(int nAxisNo, double InitPos, double InitVel, double InitAccel, double InitDecel);
                // 파라메타 28 - 31번까지 사용자가 프로그램내에서  이 함수를 이용해 확인 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetParaLoad(int nAxisNo, ref double InitPos, ref double InitVel, ref double InitAccel, ref double InitDecel);

                // 지정 축의 펄스 출력 방식을 설정한다.
                //uMethod  0 :OneHighLowHigh, 1 :OneHighHighLow, 2 :OneLowLowHigh, 3 :OneLowHighLow, 4 :TwoCcwCwHigh
                //         5 :TwoCcwCwLow, 6 :TwoCwCcwHigh, 7 :TwoCwCcwLow, 8 :TwoPhase, 9 :TwoPhaseReverse
                //    OneHighLowHigh        = 0x0,            // 1펄스 방식, PULSE(Active High), 정방향(DIR=Low)  / 역방향(DIR=High)
                //    OneHighHighLow        = 0x1,            // 1펄스 방식, PULSE(Active High), 정방향(DIR=High) / 역방향(DIR=Low)
                //    OneLowLowHigh        = 0x2,            // 1펄스 방식, PULSE(Active Low),  정방향(DIR=Low)  / 역방향(DIR=High)
                //    OneLowHighLow        = 0x3,            // 1펄스 방식, PULSE(Active Low),  정방향(DIR=High) / 역방향(DIR=Low)
                //    TwoCcwCwHigh        = 0x4,            // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active High     
                //    TwoCcwCwLow            = 0x5,            // 2펄스 방식, PULSE(CCW:역방향),  DIR(CW:정방향),  Active Low     
                //    TwoCwCcwHigh        = 0x6,            // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active High
                //    TwoCwCcwLow            = 0x7,            // 2펄스 방식, PULSE(CW:정방향),   DIR(CCW:역방향), Active Low
                //    TwoPhase            = 0x8,            // 2상(90' 위상차),  PULSE lead DIR(CW: 정방향), PULSE lag DIR(CCW:역방향)
                //    TwoPhaseReverse        = 0x9            // 2상(90' 위상차),  PULSE lead DIR(CCW: 정방향), PULSE lag DIR(CW:역방향)

                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetPulseOutMethod(int nAxisNo, uint uMethod);
                // 지정 축의 펄스 출력 방식 설정을 반환한다,
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetPulseOutMethod(int nAxisNo, ref uint upMethod);

                // 지정 축의 외부(Actual) 카운트의 증가 방향 설정을 포함하여 지정 축의 Encoder 입력 방식을 설정한다.
                // uMethod : 0 - 7 설정.
                // ObverseUpDownMode    = 0x0,            // 정방향 Up/Down
                // ObverseSqr1Mode        = 0x1,            // 정방향 1체배
                // ObverseSqr2Mode        = 0x2,            // 정방향 2체배
                // ObverseSqr4Mode      = 0x3,            // 정방향 4체배
                // ReverseUpDownMode    = 0x4,            // 역방향 Up/Down
                // ReverseSqr1Mode         = 0x5,            // 역방향 1체배
                // ReverseSqr2Mode         = 0x6,            // 역방향 2체배
                // ReverseSqr4Mode         = 0x7            // 역방향 4체배
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetEncInputMethod(int nAxisNo, uint uMethod);
                // 지정 축의 외부(Actual) 카운트의 증가 방향 설정을 포함하여 지정 축의 Encoder 입력 방식을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetEncInputMethod(int nAxisNo, ref uint upMethod);

                // 설정 속도 단위가 RPM(Revolution Per Minute)으로 맞추고 싶다면.
                // ex>    rpm 계산:
                // 4500 rpm ?
                // unit/ pulse = 1 : 1이면      pulse/ sec 초당 펄스수가 되는데
                // 4500 rpm에 맞추고 싶다면     4500 / 60 초 : 75회전/ 1초
                // 모터가 1회전에 몇 펄스인지 알아야 된다. 이것은 Encoder에 Z상을 검색해보면 알수있다.
                // 1회전:1800 펄스라면 75 x 1800 = 135000 펄스가 필요하게 된다.
                // AxmMotSetMoveUnitPerPulse에 Unit = 1, Pulse = 1800 넣어 동작시킨다.
                // 주의할점 : rpm으로 제어하게 된다면 속도와 가속도 도 rpm단위로 바뀌게 된다.

                // 지정 축의 펄스 당 움직이는 거리를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetMoveUnitPerPulse(int nAxisNo, double dUnit, int nPulse);
                // 지정 축의 펄스 당 움직이는 거리를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetMoveUnitPerPulse(int nAxisNo, ref double dpUnit, ref int npPulse);

                // 지정 축에 감속 시작 포인트 검출 방식을 설정한다.
                //uMethod : 0 -1 설정
                // AutoDetect = 0x0 : 자동 가감속.
                // RestPulse  = 0x1 : 수동 가감속.

                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetDecelMode(int nAxisNo, uint uMethod);
                // 지정 축의 감속 시작 포인트 검출 방식을 반환한다    
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetDecelMode(int nAxisNo, ref uint upMethod);

                // 지정 축에 수동 감속 모드에서 잔량 펄스를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetRemainPulse(int nAxisNo, uint uData);
                // 지정 축의 수동 감속 모드에서 잔량 펄스를 반환한다.    
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetRemainPulse(int nAxisNo, ref uint upData);

                // 지정 축에 등속도 구동 함수에서의 최고 속도를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetMaxVel(int nAxisNo, double dVel);
                // 지정 축의 등속도 구동 함수에서의 최고 속도를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetMaxVel(int nAxisNo, ref double dpVel);

                // 지정 축의 이동 거리 계산 모드를 설정한다.
                //uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
                //              POS_REL_MODE '1' - 상대 좌표계
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetAbsRelMode(int nAxisNo, uint uAbsRelMode);
                // 지정 축의 설정된 이동 거리 계산 모드를 반환한다
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetAbsRelMode(int nAxisNo, ref uint upAbsRelMode);

                //지정 축의 구동 속도 프로파일 모드를 설정한다.
                //ProfileMode : SYM_TRAPEZOIDE_MODE    '0' - 대칭 Trapezode
                //              ASYM_TRAPEZOIDE_MODE   '1' - 비대칭 Trapezode
                //              QUASI_S_CURVE_MODE     '2' - 대칭 Quasi-S Curve
                //              SYM_S_CURVE_MODE       '3' - 대칭 S Curve
                //              ASYM_S_CURVE_MODE      '4' - 비대칭 S Curve
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetProfileMode(int nAxisNo, uint uProfileMode);
                // 지정 축의 설정한 구동 속도 프로파일 모드를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetProfileMode(int nAxisNo, ref uint upProfileMode);

                //지정 축의 가속도 단위를 설정한다.
                //AccelUnit : UNIT_SEC2   '0' - 가감속 단위를 unit/sec2 사용
                //            SEC         '1' - 가감속 단위를 sec 사용
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetAccelUnit(int nAxisNo, uint uAccelUnit);
                // 지정 축의 설정된 가속도단위를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetAccelUnit(int nAxisNo, ref uint upAccelUnit);

                // 지정 축에 초기 속도를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetMinVel(int nAxisNo, double dMinVelocity);
                // 지정 축의 초기 속도를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetMinVel(int nAxisNo, ref double dpMinVelocity);

                // 지정 축의 가속 저크값을 설정한다.[%].
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetAccelJerk(int nAxisNo, double dAccelJerk);
                // 지정 축의 설정된 가속 저크값을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetAccelJerk(int nAxisNo, ref double dpAccelJerk);

                // 지정 축의 감속 저크값을 설정한다.[%].
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetDecelJerk(int nAxisNo, double dDecelJerk);
                // 지정 축의 설정된 감속 저크값을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetDecelJerk(int nAxisNo, ref double dpDecelJerk);

                // 지정 축의 속도 Profile결정시 우선순위(속도 Or 가속도)를 설정한다.
                // Priority : PRIORITY_VELOCITY   '0' - 속도 Profile결정시 지정한 속도값에 가깝도록 계산함(일반장비 및 Spinner에 사용).
                //           PRIORITY_ACCELTIME  '1' - 속도 Profile결정시 지정한 가감속시간에 가깝도록 계산함(고속 장비에 사용).
                [DllImport("AXL.dll")]
                public static extern uint AxmMotSetProfilePriority(int nAxisNo, uint uPriority);
                // 지정 축의 속도 Profile결정시 우선순위(속도 Or 가속도)를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMotGetProfilePriority(int nAxisNo, ref uint upPriority);

                //=========== 입출력 신호 관련 설정함수 ================================================================================
                // 지정 축의 Z 상 Level을 설정한다.
                // uLevel : LOW(0), HIGH(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetZphaseLevel(int nAxisNo, uint uLevel);
                // 지정 축의 Z 상 Level을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetZphaseLevel(int nAxisNo, ref uint upLevel);

                // 지정 축의 Servo-On신호의 출력 레벨을 설정한다.
                // uLevel : LOW(0), HIGH(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetServoOnLevel(int nAxisNo, uint uLevel);
                // 지정 축의 Servo-On신호의 출력 레벨 설정을 반환한다.    
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetServoOnLevel(int nAxisNo, ref uint upLevel);

                // 지정 축의 Servo-Alarm Reset 신호의 출력 레벨을 설정한다.
                // uLevel : LOW(0), HIGH(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetServoAlarmResetLevel(int nAxisNo, uint uLevel);
                // 지정 축의 Servo-Alarm Reset 신호의 출력 레벨을 설정을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetServoAlarmResetLevel(int nAxisNo, ref uint upLevel);

                // 지정 축의 Inpositon 신호 사용 여부 및 신호 입력 레벨을 설정한다
                // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)    
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetInpos(int nAxisNo, uint uUse);
                // 지정 축의 Inpositon 신호 사용 여부 및 신호 입력 레벨을 반환한다.    
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetInpos(int nAxisNo, ref uint upUse);
                // 지정 축의 Inpositon 신호 입력 상태를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadInpos(int nAxisNo, ref uint upStatus);

                // 지정 축의 알람 신호 입력 시 비상 정지의 사용 여부 및 신호 입력 레벨을 설정한다.
                // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetServoAlarm(int nAxisNo, uint uUse);
                // 지정 축의 알람 신호 입력 시 비상 정지의 사용 여부 및 신호 입력 레벨을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetServoAlarm(int nAxisNo, ref uint upUse);
                // 지정 축의 알람 신호의 입력 레벨을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadServoAlarm(int nAxisNo, ref uint upStatus);

                // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨을 설정한다. 
                // end limit sensor 신호 입력 시 감속정지 또는 급정지에 대한 설정도 가능하다.
                // uStopMode: EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
                // uPositiveLevel, uNegativeLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetLimit(int nAxisNo, uint uStopMode, uint uPositiveLevel, uint uNegativeLevel);
                // 지정 축의 end limit sensor의 사용 유무 및 신호의 입력 레벨, 신호 입력 시 정지모드를 반환한다
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetLimit(int nAxisNo, ref uint upStopMode, ref uint upPositiveLevel, ref uint upNegativeLevel);
                // 지정축의 end limit sensor의 입력 상태를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadLimit(int nAxisNo, ref uint upPositiveStatus, ref uint upNegativeStatus);

                // 지정 축의 Software limit의 사용 유무, 사용할 카운트, 그리고 정지 방법을 설정한다
                // uUse       : DISABLE(0), ENABLE(1)
                // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
                // uSelection : COMMAND(0), ACTUAL(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetSoftLimit(int nAxisNo, uint uUse, uint uStopMode, uint uSelection, double dPositivePos, double dNegativePos);
                // 지정 축의 Software limit의 사용 유무, 사용할 카운트, 그리고 정지 방법을 반환한다
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetSoftLimit(int nAxisNo, ref uint upUse, ref uint upStopMode, ref uint upSelection, ref double dpPositivePos, ref double dpNegativePos);

                // 비상 정지 신호의 정지 방법 (급정지/감속정지) 또는 사용 유무를 설정한다.
                // uStopMode  : EMERGENCY_STOP(0), SLOWDOWN_STOP(1)
                // uLevel : LOW(0), HIGH(1), UNUSED(2), USED(3)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalSetStop(int nAxisNo, uint uStopMode, uint uLevel);
                // 비상 정지 신호의 정지 방법 (급정지/감속정지) 또는 사용 유무를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalGetStop(int nAxisNo, ref uint upStopMode, ref uint upLevel);
                // 비상 정지 신호의 입력 상태를 반환한다.    
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadStop(int nAxisNo, ref uint upStatus);

                // 지정 축의 Servo-On 신호를 출력한다.
                // uOnOff : FALSE(0), TRUE(1) ( 범용 0출력에 해당됨)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalServoOn(int nAxisNo, uint uUse);
                // 지정 축의 Servo-On 신호의 출력 상태를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalIsServoOn(int nAxisNo, ref uint upUse);

                // 지정 축의 Servo-Alarm Reset 신호를 출력한다.
                // uOnOff : FALSE(0), TRUE(1) ( 범용 1출력에 해당됨)
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalServoAlarmReset(int nAxisNo, uint uOnOff);

                // 범용 출력값을 설정한다.
                // uValue : Hex Value 0x00
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalWriteOutput(int nAxisNo, uint uValue);
                // 범용 출력값을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadOutput(int nAxisNo, ref uint upValue);

                // lBitNo : Bit Number(0 - 4)
                // uOnOff : FALSE(0), TRUE(1)
                // 범용 출력값을 비트별로 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalWriteOutputBit(int nAxisNo, int nBitNo, uint uOn);
                // 범용 출력값을 비트별로 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadOutputBit(int nAxisNo, int nBitNo, ref uint upOn);

                // 범용 입력값을 Hex값으로 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadInput(int nAxisNo, ref uint upValue);

                // lBitNo : Bit Number(0 - 4)
                // 범용 입력값을 비트별로 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSignalReadInputBit(int nAxisNo, int nBitNo, ref uint upOn);

                //========== 모션 구동중 및 구동후에 상태 확인하는 함수============================================================

                // "모션 구동 중인가를 확인
                // (구동상태)"
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadInMotion(int nAxisNo, ref uint upStatus);

                //  "구동시작 이후 출력된 펄스 카운터 값을 확인
                //  (펄스 카운트 값)"
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadDrivePulseCount(int nAxisNo, ref int npPulse);

                // DriveStatus 레지스터를 확인
                // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadMotion(int nAxisNo, ref uint upStatus);

                // EndStatus 레지스터를 확인
                // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadStop(int nAxisNo, ref uint upStatus);

                // Mechanical 레지스터를 확인
                // 주의사항 : 각 제품별로 하드웨어적인 신호가 다르기때문에 매뉴얼 및 AXHS.xxx 파일을 참고해야한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadMechanical(int nAxisNo, ref uint upStatus);

                // 현재 속도를 읽어 온다
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadVel(int nAxisNo, ref double dpVelocity);

                // Command Pos과 Actual Pos의 차를 확인
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadPosError(int nAxisNo, ref double dpError);

                // 최후 드라이브의 이동 거리를 확인
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusReadDriveDistance(int nAxisNo, ref double dpUnit);

                // 지정 축의 Actual 위치를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusSetActPos(int nAxisNo, double dPos);
                // 지정 축의 Actual 위치를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusGetActPos(int nAxisNo, ref double dpPos);

                // 지정 축의 Command 위치를 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusSetCmdPos(int nAxisNo, double dPos);
                // 지정 축의 Command 위치를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmStatusGetCmdPos(int nAxisNo, ref double dpPos);


                //======== 홈관련 함수=============================================================================================================================================================================================    

                // 지정 축의 Home 센서 Level 을 설정한다.
                // uLevel : LOW(0), HIGH(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeSetSignalLevel(int nAxisNo, uint uLevel);
                // 지정 축의 Home 센서 Level 을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeGetSignalLevel(int nAxisNo, ref uint upLevel);
                // 현재 홈 신호 입력상태를 확인한다. 홈신호는 사용자가 임의로 AxmHomeSetMethod 함수를 이용하여 설정할수있다.
                // upStatus : OFF(0), ON(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeReadSignal(int nAxisNo, ref uint upStatus);

                // 해당 축의 원점검색을 수행하기 위해서는 반드시 원점 검색관련 파라메타들이 설정되어 있어야 됩니다. 
                // 만약 MotionPara설정 파일을 이용해 초기화가 정상적으로 수행됐다면 별도의 설정은 필요하지 않다. 
                // 원점검색 방법 설정에는 검색 진행방향, 원점으로 사용할 신호, 원점센서 Active Level, 엔코더 Z상 검출 여부 등을 설정 한다.
                // (자세한 내용은 AxmMotSaveParaAll 설명 부분 참조)
                // 홈레벨은 AxmSignalSetHomeLevel 사용한다.
                // HClrTim : HomeClear Time : 원점 검색 Encoder 값 Set하기 위한 대기시간 
                // HmDir(홈 방향): DIR_CCW (0) -방향 , DIR_CW(1) +방향
                // HOffset - 원점검출후 이동거리.
                // uZphas: 1차 원점검색 완료 후 엔코더 Z상 검출 유무 설정  0: 사용안함 , 1: +방향, 2: -방향 
                // HmSig : PosEndLimit(0) -> +Limit
                //         NegEndLimit(1) -> -Limit
                //         HomeSensor (4) -> 원점센서(범용 입력 0)

                [DllImport("AXL.dll")]
                public static extern uint AxmHomeSetMethod(int nAxisNo, int nHmDir, uint uHomeSignal, uint uZphas, double dHomeClrTime, double dHomeOffset);
                // 설정되어있는 홈 관련 파라메타들을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeGetMethod(int nAxisNo, ref int nHmDir, ref uint uHomeSignal, ref uint uZphas, ref double dHomeClrTime, ref double dHomeOffset);

                // 원점을 빠르고 정밀하게 검색하기 위해 여러 단계의 스탭으로 검출한다. 이때 각 스탭에 사용 될 속도를 설정한다. 
                // 이 속도들의 설정값에 따라 원점검색 시간과, 원점검색 정밀도가 결정된다. 
                // 각 스탭별 속도들을 적절히 바꿔가면서 각 축의 원점검색 속도를 설정하면 된다. 
                // (자세한 내용은 AxmMotSaveParaAll 설명 부분 참조)
                // 원점검색시 사용될 속도를 설정하는 함수
                // [dVelFirst]- 1차구동속도   [dVelSecond]-검출후속도   [dVelThird]- 마지막 속도  [dvelLast]- index검색및 정밀하게 검색하기위해. 
                // [dAccFirst]- 1차구동가속도 [dAccSecond]-검출후가속도 
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeSetVel(int nAxisNo, double dVelFirst, double dVelSecond, double dVelThird, double dvelLast, double dAccFirst, double dAccSecond);
                // 설정되어있는 원점검색시 사용될 속도를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeGetVel(int nAxisNo, ref double dVelFirst, ref double dVelSecond, ref double dVelThird, ref double dvelLast, ref double dAccFirst, ref double dAccSecond);

                // 원점검색을 시작한다.
                // 원점검색 시작함수를 실행하면 라이브러리 내부에서 해당축의 원점검색을 수행 할 쓰레드가 자동 생성되어 원점검색을 순차적으로 수행한 후 자동 종료된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeSetStart(int nAxisNo);
                // 원점검색 결과를 사용자가 임의로 설정한다.
                // 원점검색 함수를 이용해 성공적으로 원점검색이 수행되고나면 검색 결과가 HOME_SUCCESS로 설정됩니다.
                // 이 함수는 사용자가 원점검색을 수행하지않고 결과를 임의로 설정할 수 있다. 
                // uHomeResult 설정
                // HOME_SUCCESS                    = 0x01    // 홈 완료
                // HOME_SEARCHING                = 0x02    // 홈검색중
                // HOME_ERR_GNT_RANGE            = 0x10    // 홈 검색 범위를 벗어났을경우
                // HOME_ERR_USER_BREAK            = 0x11    // 속도 유저가 임의로 정지명령을 내렸을경우
                // HOME_ERR_VELOCITY            = 0x12    // 속도 설정 잘못했을경우
                // HOME_ERR_AMP_FAULT            = 0x13    // 서보팩 알람 발생 에러
                // HOME_ERR_NEG_LIMIT            = 0x14    // (-)방향 구동중 (+)리미트 센서 감지 에러
                // HOME_ERR_POS_LIMIT            = 0x15    // (+)방향 구동중 (-)리미트 센서 감지 에러
                // HOME_ERR_NOT_DETECT            = 0x16    // 지정한 신호 검출하지 못 할 경우 에러
                // HOME_ERR_UNKNOWN                = 0xFF    
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeSetResult(int nAxisNo, uint uHomeResult);
                // 원점검색 결과를 반환한다.
                // 원점검색 함수의 검색 결과를 확인한다. 원점검색이 시작되면 HOME_SEARCHING으로 설정되며 원점검색에 실패하면 실패원인이 설정된다. 실패 원인을 제거한 후 다시 원점검색을 진행하면 된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeGetResult(int nAxisNo, ref uint upHomeResult);
                // 원점검색 진행률을 반환한다.
                // 원점검색 시작되면 진행율을 확인할 수 있다. 원점검색이 완료되면 성공여부와 관계없이 100을 반환하게 된다. 원점검색 성공여부는 GetHome Result함수를 이용해 확인할 수 있다.
                // upHomeMainStepNumber : Main Step 진행율이다. 
                // 겐트리 FALSE일 경우upHomeMainStepNumber : 0 일때면 선택한 축만 진행사항이고 홈 진행율은 upHomeStepNumber 표시한다.
                // 겐트리 TRUE일 경우 upHomeMainStepNumber : 0 일때면 마스터 홈을 진행사항이고 마스터 홈 진행율은 upHomeStepNumber 표시한다.
                // 겐트리 TRUE일 경우 upHomeMainStepNumber : 10 일때면 슬레이브 홈을 진행사항이고 마스터 홈 진행율은 upHomeStepNumber 표시한다.
                // upHomeStepNumber     : 선택한 축에대한 진행율을 표시한다. 
                // 겐트리 FALSE일 경우  : 선택한 축만 진행율을 표시한다.
                // 겐트리 TRUE일 경우 마스터축, 슬레이브축 순서로 진행율을 표시된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmHomeGetRate(int nAxisNo, ref uint upHomeMainStepNumber, ref uint upHomeStepNumber);

                //========= 위치 구동함수 ===============================================================================================================

                // 설정 속도 단위가 RPM(Revolution Per Minute)으로 맞추고 싶다면.
                // ex>    rpm 계산:
                // 4500 rpm ?
                // unit/ pulse = 1 : 1이면      pulse/ sec 초당 펄스수가 되는데
                // 4500 rpm에 맞추고 싶다면     4500 / 60 초 : 75회전/ 1초
                // 모터가 1회전에 몇 펄스인지 알아야 된다. 이것은 Encoder에 Z상을 검색해보면 알수있다.
                // 1회전:1800 펄스라면 75 x 1800 = 135000 펄스가 필요하게 된다.
                // AxmMotSetMoveUnitPerPulse에 Unit = 1, Pulse = 1800 넣어 동작시킨다. 

                // 설정한 거리만큼 또는 위치까지 이동한다.
                // 지정 축의 절대 좌표/ 상대좌표 로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
                // 속도 프로파일은 AxmMotSetProfileMode 함수에서 설정한다.
                // 펄스가 출력되는 시점에서 함수를 벗어난다.
                // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
                // AxmMotSetAccelUnit(lAxisNo, 1) 일경우 dAccel -> dAccelTime , dDecel -> dDecelTime 으로 바뀐다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveStartPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

                // 설정한 거리만큼 또는 위치까지 이동한다.
                // 지정 축의 절대 좌표/상대좌표로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
                // 속도 프로파일은 AxmMotSetProfileMode 함수에서 설정한다. 
                // 펄스 출력이 종료되는 시점에서 함수를 벗어난다
                // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
                [DllImport("AXL.dll")]
                public static extern uint AxmMovePos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel);

                // 설정한 속도로 구동한다.
                // 지정 축에 대하여 설정된 속도와 가속율로 지속적으로 속도 모드 구동을 한다. 
                // 펄스 출력이 시작되는 시점에서 함수를 벗어난다.
                // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveVel(int nAxisNo, double dVel, double dAccel, double dDecel);

                // 지정된 다축에 대하여 설정된 속도와 가속율로 지속적으로 속도 모드 구동을 한다.
                // 펄스 출력이 시작되는 시점에서 함수를 벗어난다.
                // PCI-Nx04 제품만 함수사용가능.
                // SMC-2V03 module 경우 2축만 사용가능.
                // Vel값이 양수이면 CW, 음수이면 CCW 방향으로 구동.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveStartMultiVel(int lArraySize, ref int lpAxesNo, ref double dVel, ref double dAccel, ref double dDecel);

                // 특정 Input 신호의 Edge를 검출하여 즉정지 또는 감속정지하는 함수.
                // lDetect Signal : edge 검출할 입력 신호 선택.
                // lDetectSignal  : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
                // Signal Edge    : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
                //                  SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
                // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
                // SignalMethod  : 급정지 EMERGENCY_STOP(0), 감속정지 SLOWDOWN_STOP(1)
                // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
                //           PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveSignalSearch(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nSignalMethod);

                // 지정 축에서 설정된 신호를 검출하고 그 위치를 저장하기 위해 이동하는 함수이다.
                // 원하는 신호를 골라 찾아 움직이는 함수 찾을 경우 그 위치를 저장시켜놓고 AxmGetCapturePos사용하여 그값을 읽는다.
                // Signal Edge   : 선택한 입력 신호의 edge 방향 선택 (rising or falling edge).
                //                 SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
                // 구동방향      : Vel값이 양수이면 CW, 음수이면 CCW.
                // SignalMethod  : 급정지 EMERGENCY_STOP(0), 감속정지 SLOWDOWN_STOP(1)
                // lDetect Signal: edge 검출할 입력 신호 선택.SIGNAL_DOWN_EDGE(0), SIGNAL_UP_EDGE(1)
                // lDetectSignal : PosEndLimit(0), NegEndLimit(1), HomeSensor(4), EncodZPhase(5), UniInput02(6), UniInput03(7)
                // lTarget       : COMMAND(0), ACTUAL(1)
                // 주의사항: SignalMethod를 EMERGENCY_STOP(0)로 사용할경우 가감속이 무시되며 지정된 속도로 가속 급정지하게된다.
                //           PCI-Nx04를 사용할 경우 lDetectSignal이 PosEndLimit , NegEndLimit(0,1) 을 찾을경우 신호의레벨 Active 상태를 검출하게된다.
                //           SMC-2V03모듈 IP의 경우 한축만 동작 가능하며 한축 이상 구동할경우 위치가 저장이 안된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveSignalCapture(int nAxisNo, double dVel, double dAccel, int nDetectSignal, int nSignalEdge, int nTarget, int nSignalMethod);

                // 'AxmMoveSignalCapture' 함수에서 저장된 위치값을 확인하는 함수이다.
                // 주의사항: 함수 실행 결과가 "AXT_RT_SUCCESS"일때 저장된 위치가 유효하며, 이 함수를 한번 실행하면 저장 위치값이 초기화된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveGetCapturePos(int nAxisNo, ref double dpCapPos);

                // "설정한 거리만큼 또는 위치까지 이동하는 함수.
                // 함수를 실행하면 해당 Motion 동작을 시작한 후 Motion 이 완료될때까지 기다리지 않고 바로 함수를 빠져나간다."
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveStartMultiPos(int nArraySize, ref int nAxisNo, ref double dPos, ref double dVel, ref double dAccel, ref double dDecel);

                // 다축을 설정한 거리만큼 또는 위치까지 이동한다.
                // 지정 축들의 절대 좌표로 설정된 위치까지 설정된 속도와 가속율로 구동을 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveMultiPos(int nArraySize, ref int nAxisNo, ref double dPos, ref double dVel, ref double dAccel, ref double dDecel);

                // 지정 축을 설정한 감속도로 감속 정지 한다.
                // dDecel : 정지 시 감속율값
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveStop(int nAxisNo, double dDecel);
                // 지정 축을 급 정지 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveEStop(int nAxisNo);
                // 지정 축을 감속 정지한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMoveSStop(int nAxisNo);

                //========= 오버라이드 함수 ============================================================================

                // 위치 오버라이드 한다.
                // 지정 축의 구동이 종료되기 전 지정된 출력 펄스 수를 조정한다.
                // PCI-Nx04 사용시주의사항: 오버라이드할 위치를 넣을때는 구동 시점의 위치를 기준으로한 Relative 형태의 위치값으로 넣어준다.
                //                          구동시작후 같은방향의 경우 오버라이드를 계속할수있지만 반대방향으로 오버라이드할경우에는 오버라이드를 계속할수없다.
                [DllImport("AXL.dll")]
                public static extern uint AxmOverridePos(int nAxisNo, double dOverridePos);

                // 지정 축의 속도오버라이드 하기전에 오버라이드할 최고속도를 설정한다.
                // 주의점 : 속도오버라이드를 5번한다면 그중에 최고 속도를 설정해야된다. 
                [DllImport("AXL.dll")]
                public static extern uint AxmOverrideSetMaxVel(int nAxisNo, double dOverrideMaxVel);

                // 속도 오버라이드 한다.
                // 지정 축의 구동 중에 속도를 가변 설정한다. (반드시 모션 중에 가변 설정한다.)
                // 주의점: AxmOverrideVel 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
                // EX> 속도오버라이드를 두번한다면 
                // 1. 두개중에 높은 속도를 AxmOverrideMaxVel 설정 최고 속도값 설정.
                // 2. AxmMoveStartPos 실행 지정 축의 구동 중(Move함수 모두 포함)에 속도를 첫번째 속도로 AxmOverrideVel 가변 설정한다.
                // 3. 지정 축의 구동 중(Move함수 모두 포함)에 속도를 두번째 속도로 AxmOverrideVel 가변 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmOverrideVel(int nAxisNo, double dOverrideVelocity);

                // SMC-2V03 module은 지원안함. PCI-Nx04 만 지원함.
                // 가속도, 속도, 감속도를  오버라이드 한다.
                // 지정 축의 구동 중에 가속도, 속도, 감속도를 가변 설정한다. (반드시 모션 중에 가변 설정한다.)
                // 주의점: AxmOverrideAccelVelDecel 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
                // EX> 속도오버라이드를 두번한다면 
                // 1. 두개중에 높은 속도를 AxmOverrideMaxVel 설정 최고 속도값 설정.
                // 2. AxmMoveStartPos 실행 지정 축의 구동 중(Move함수 모두 포함)에 가속도, 속도, 감속도를 첫번째 속도로 AxmOverrideAccelVelDecel 가변 설정한다.
                // 3. 지정 축의 구동 중(Move함수 모두 포함)에 가속도, 속도, 감속도를 두번째 속도로 AxmOverrideAccelVelDecel 가변 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmOverrideAccelVelDecel(int nAxisNo, double dOverrideVelocity, double dMaxAccel, double dMaxDecel);

                // 어느 시점에서 속도 오버라이드 한다.
                // 어느 위치 지점과 오버라이드할 속도를 입력시켜 그위치에서 속도오버라이드 되는 함수
                // lTarget : COMMAND(0), ACTUAL(1)
                // 주의점: AxmOverrideVelAtPos 함수를 사용하기전에. AxmOverrideMaxVel 최고로 설정할수있는 속도를 설정해놓는다.
                [DllImport("AXL.dll")]
                public static extern uint AxmOverrideVelAtPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, double dOverridePos, double dOverrideVelocity, int nTarget);

                // 지정한 시점들에서 지정한 속도로 오버라이드 한다.
                // lArraySize     : 오버라이드 할 위치의 개수를 설정.
                // *dpOverridePos : 오버라이드 할 위치의 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
                // *dpOverrideVel : 오버라이드 할 위치에서 변경 될 속도 배열(lArraySize에서 설정한 개수보다 같거나 크게 선언해야됨)
                // lTarget        : COMMAND(0), ACTUAL(1) 
                // dwOverrideMode : 오버라이드 시작 방법을 지정함.
                //                : OVERRIDE_POS_START(0) 지정한 위치에서 지정한 속도로 오버라이드 시작함        
                //                : OVERRIDE_POS_END(1) 지정한 위치에서 지정한 속도가 되도록 미리 오버라이드 시작함
                [DllImport("AXL.dll")]
                public static extern uint AxmOverrideVelAtMultiPos(int nAxisNo, double dPos, double dVel, double dAccel, double dDecel, int nArraySize, ref double dpOverridePos, ref double dpOverrideVel, int nTarget, uint uOverrideMode);

                //========= 마스터, 슬레이브  기어비로 구동 함수 ===========================================================================

                // Electric Gear 모드에서 Master 축과 Slave 축과의 기어비를 설정한다.
                // dSlaveRatio : 마스터축에 대한 슬레이브의 기어비( 0 : 0% , 0.5 : 50%, 1 : 100%)
                [DllImport("AXL.dll")]
                public static extern uint AxmLinkSetMode(int nMasterAxisNo, int nSlaveAxisNo, double dSlaveRatio);
                // Electric Gear 모드에서 설정된 Master 축과 Slave 축과의 기어비를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmLinkGetMode(int nMasterAxisNo, ref uint nSlaveAxisNo, ref double dpGearRatio);
                // Master 축과 Slave축간의 전자기어비를 설정 해제 한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmLinkResetMode(int nMasterAxisNo);

                //======== 겐트리 관련 함수==========================================================================================================================================================
                // 모션모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 지원한다. 
                // 이 함수를 이용해 Master축을 겐트리 제어로 설정하면 해당 Slave축은 Master축과 동기되어 구동됩니다. 
                // 만약 겐트리 설정 이후 Slave축에 구동명령이나 정지 명령등을 내려도 모두 무시됩니다.
                // uSlHomeUse     : 슬레이축 홈사용 우뮤 ( 0 - 2)
                //             (0 : 슬레이브축 홈을 사용안하고 마스터축을 홈을 찾는다.)
                //             (1 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정함.)
                //             (2 : 마스터축 , 슬레이브축 홈을 찾는다. 슬레이브 dSlOffset 값 적용해서 보정안함.)
                // dSlOffset      : 슬레이브축 옵셋값
                // dSlOffsetRange : 슬레이브축 옵셋값 레인지 설정
                // PCI-Nx04 사용시주의사항: 갠트리 ENABLE시 슬레이브축은 모션중 AxmStatusReadMotion 함수로 확인하면 True(Motion 구동 중)로 확인되야 정상동작이다. 
                //                   슬레이브축에 AxmStatusReadMotion로 확인했을때 InMotion 이 False이면 Gantry Enable이 안된것이므로 알람 혹은 리밋트 센서 등을 확인한다.

                [DllImport("AXL.dll")]
                public static extern uint AxmGantrySetEnable(int nMasterAxisNo, int nSlaveAxisNo, uint uSlHomeUse, double dSlOffset, double dSlOffsetRange);

                // Slave축의 Offset값을 알아내는방법.
                // A. 마스터, 슬레이브를 두개다 서보온을 시킨다.         
                // B. AxmGantrySetEnable함수에서 uSlHomeUse = 2로 설정후 AxmHomeSetStart함수를 이용해서 홈을 찾는다. 
                // C. 홈을 찾고 나면 마스터축의 Command값을 읽어보면 마스터축과 슬레이브축의 틀어진 Offset값을 볼수있다.
                // D. Offset값을 읽어서 AxmGantrySetEnable함수의 dSlOffset인자에 넣어준다. 
                // E. dSlOffset값을 넣어줄때 마스터축에 대한 슬레이브 축 값이기때문에 부호를 반대로 -dSlOffset 넣어준다.
                // F. dSIOffsetRange 는 Slave Offset의 Range 범위를 말하는데 Range의 한계를 지정하여 한계를 벗어나면 에러를 발생시킬때 사용한다.        
                // G. AxmGantrySetEnable함수에 Offset값을 넣어줬으면  AxmGantrySetEnable함수에서 uSlHomeUse = 1로 설정후 AxmHomeSetStart함수를 이용해서 홈을 찾는다.         

                // 겐트리 구동에 있어 사용자가 설정한 파라메타를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmGantryGetEnable(int nMasterAxisNo, ref uint upSlHomeUse, ref double dpSlOffset, ref double dSlORange, ref uint uGatryOn);

                // 모션 모듈은 두 축이 기구적으로 Link되어있는 겐트리 구동시스템 제어를 해제한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmGantrySetDisable(int nMasterAxisNo, int nSlaveAxisNo);

                //====일반 보간함수 ============================================================================================================================================;

                // 주의사항1: AxmContiSetAxisMap함수를 이용하여 축맵핑후에 낮은순서축부터 맵핑을 하면서 사용해야된다.
                //           원호보간의 경우에는 반드시 낮은순서축부터 축배열에 넣어야 동작 가능하다.

                // 주의사항2: 위치를 설정할경우 반드시 마스터축과 슬레이브 축의 UNIT/PULSE의 맞추어서 설정한다.
                //           위치를 UNIT/PULSE 보다 작게 설정할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치까지 구동이 될수없다.

                // 주의사항3: 원호 보간을 할경우 반드시 한칩내에서 구동이 될수있으므로 
                //            SMC-2V03 모듈은 2축만 가능며 N404, N804 보드는 4축내에서만 선택해서 사용해야된다.

                // 주의사항4: 보간 구동 시작/중에 비정상 정지 조건(+- Limit신호, 서보 알람, 비상정지 등)이 발생하면 
                //            구동 방향에 상관없이 구동을 시작하지 않거나 정지 된다.

                // 직선 보간 한다.
                // 시작점과 종료점을 지정하여 다축 직선 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점과 종료점을 지정하여 직선 보간 구동하는 Queue에 저장함수가된다. 
                // 직선 프로파일 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmLineMove(int lCoord, ref double dPos, double dVel, double dAccel, double dDecel);

                // 2축 원호보간 한다.
                // 시작점, 종료점과 중심점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
                // AxmContiBeginNode, AxmContiEndNode, 와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
                // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
                // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dEndPos = 종료점 X,Y 배열.
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향

                [DllImport("AXL.dll")]
                public static extern uint AxmCircleCenterMove(int lCoord, ref int lAxisNo, ref double dCenterPos, ref double dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir);
                // 중간점, 종료점을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 구동하는 원호 보간 Queue에 저장함수가된다.
                // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
                // lAxisNo = 두축 배열 , dMidPos = 중간점 X,Y 배열 , dEndPos = 종료점 X,Y 배열, lArcCircle = 아크(0), 원(1)
                [DllImport("AXL.dll")]
                public static extern uint AxmCirclePointMove(int lCoord, ref int lAxisNo, ref double dMidPos, ref double dEndPos, double dVel, double dAccel, double dDecel);
                // 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
                // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
                // lAxisNo = 두축 배열 , dRadius = 반지름, dEndPos = 종료점 X,Y 배열 , uShortDistance = 작은원(0), 큰원(1)
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향

                [DllImport("AXL.dll")]
                public static extern uint AxmCircleRadiusMove(int lCoord, ref int lAxisNo, double dRadius, ref double dEndPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);
                // 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 함수이다. 구동 시작 후 함수를 벗어난다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 원호 보간 구동하는 Queue에 저장함수가된다.
                // 프로파일 원호 연속 보간 구동을 위해 내부 Queue에 저장하여 AxmContiStart함수를 사용해서 시작한다.
                // lAxisNo = 두축 배열 , dCenterPos = 중심점 X,Y 배열 , dAngle = 각도.
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향
                [DllImport("AXL.dll")]
                public static extern uint AxmCircleAngleMove(int lCoord, ref int lAxisNo, ref double dCenterPos, double dAngle, double dVel, double dAccel, double dDecel, uint uCWDir);

                //====연속 보간 함수 ============================================================================================================================================;
                //지정된 좌표계에 연속보간 축 맵핑을 설정한다.
                //(축맵핑 번호는 0 부터 시작))
                // 주의점: 축맵핑할때는 반드시 실제 축번호가 작은 숫자부터 큰숫자를 넣는다.
                //         가상축 맵핑 함수를 사용하였을 때 가상축번호를 실제 축번호가 작은 값 부터 lpAxesNo의 낮은 인텍스에 입력하여야 한다.
                //         가상축 맵핑 함수를 사용하였을 때 가상축번호에 해당하는 실제 축번호가 다른 값이라야 한다.
                //         SMC-2V03의 경우 lSize는 2로 입력하여야 한다.
                //         같은 축을 다른 Coordinate에 중복 맵핑하지 말아야 한다.

                [DllImport("AXL.dll")]
                public static extern uint AxmContiSetAxisMap(int lCoord, uint lSize, ref int lpRealAxesNo);
                //지정된 좌표계에 연속보간 축 맵핑을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiGetAxisMap(int lCoord, ref uint lSize, ref int lpRealAxesNo);

                // 지정된 좌표계에 연속보간 축 절대/상대 모드를 설정한다.
                // (주의점 : 반드시 축맵핑 하고 사용가능)
                // 지정 축의 이동 거리 계산 모드를 설정한다.
                //uAbsRelMode : POS_ABS_MODE '0' - 절대 좌표계
                //              POS_REL_MODE '1' - 상대 좌표계

                [DllImport("AXL.dll")]
                public static extern uint AxmContiSetAbsRelMode(int lCoord, uint uAbsRelMode);
                // 지정된 좌표계에 연속보간 축 절대/상대 모드를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiGetAbsRelMode(int lCoord, ref uint upAbsRelMode);

                // 지정된 좌표계에 보간 구동을 위한 내부 Queue가 비어 있는지 확인하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiReadFree(int lCoord, ref uint upQueueFree);
                // 지정된 좌표계에 보간 구동을 위한 내부 Queue에 저장되어 있는 보간 구동 개수를 확인하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiReadIndex(int lCoord, ref int npQueueIndex);
                // 지정된 좌표계에 연속 보간 구동을 위해 저장된 내부 Queue를 모두 삭제하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiWriteClear(int lCoord);

                // 지정된 좌표계에 연속보간에서 수행할 작업들의 등록을 시작한다. 이함수를 호출한후,
                // AxmContiEndNode함수가 호출되기 전까지 수행되는 모든 모션작업은 실제 모션을 수행하는 것이 아니라 연속보간 모션으로 등록 되는 것이며,
                // AxmContiStart 함수가 호출될 때 비로소 등록된모션이 실제로 수행된다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiBeginNode(int lCoord);
                // 지정된 좌표계에서 연속보간을 수행할 작업들의 등록을 종료한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiEndNode(int lCoord);

                // 연속 보간 시작 한다.
                // SMC-2V03 module :  dwProfileset, lAngle 값을 0으로 입력함. 
                // PCI-Nx04 : dwProfileset(CONTI_NODE_VELOCITY(0) : 연속 보간 사용, CONTI_NODE_MANUAL(1) : 프로파일 보간 사용, CONTI_NODE_AUTO(2) : 자동 프로파일 보간, 3 : 속도보상 모드 사용) 
                [DllImport("AXL.dll")]
                public static extern uint AxmContiStart(int lCoord, uint dwProfileset, int lAngle);
                // 지정된 좌표계에 연속 보간 구동 중인지 확인하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiIsMotion(int lCoord, ref uint upInMotion);
                // 지정된 좌표계에 연속 보간 구동 중 현재 구동중인 연속 보간 인덱스 번호를 확인하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiGetNodeNum(int lCoord, ref int npNodeNum);
                // 지정된 좌표계에 설정한 연속 보간 구동 총 인덱스 갯수를 확인하는 함수이다.
                [DllImport("AXL.dll")]
                public static extern uint AxmContiGetTotalNodeNum(int lCoord, ref int npNodeNum);

                //====================트리거 함수 ===============================================================================================================================
                // 주의사항: 트리거 위치를 설정할경우 반드시 UNIT/PULSE의 맞추어서 설정한다.
                //           위치를 UNIT/PULSE 보다 작게할 경우 최소단위가 UNIT/PULSE로 맞추어지기때문에 그위치에 출력할수없다.

                // 지정 축에 트리거 기능의 사용 여부, 출력 레벨, 위치 비교기, 트리거 신호 지속 시간 및 트리거 출력 모드를 설정한다.
                // 트리거 기능 사용을 위해서는 먼저  AxmTriggerSetTimeLevel 를 사용하여 관련 기능 설정을 먼저 하여야 한다.
                //  dTrigTime : 트리거 출력 시간 
                //                SMC-2V03 module : 1usec - 최대 4msec ( 1 - 4000 까지 설정)
                //                PCI-Nx04 : 1usec - 최대 50msec ( 1 - 50000 까지 설정)
                //  upTriggerLevel  : 트리거 출력 레벨 유무 => LOW(0), HIGH(1)
                //  uSelect         : 사용할 기준 위치        => COMMAND(0), ACTUAL(1)
                //  uInterrupt      : 인터럽트 설정            => DISABLE(0), ENABLE(1)

                // 지정 축에 트리거 신호 지속 시간 및 트리거 출력 레벨, 트리거 출력방법을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerSetTimeLevel(int lAxisNo, double dTrigTime, uint uTriggerLevel, uint uSelect, uint uInterrupt);
                // 지정 축에 트리거 신호 지속 시간 및 트리거 출력 레벨, 트리거 출력방법을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerGetTimeLevel(int lAxisNo, ref double dTrigTime, ref uint uTriggerLevel, ref uint uSelect, ref uint uInterrupt);

                // 지정 축의 트리거 출력 기능을 설정한다.
                //  uMethod :    PERIOD_MODE      0x0 : 현재 위치를 기준으로 dPos를 위치 주기로 사용한 주기 트리거 방식
                //                ABS_POS_MODE     0x1 : 트리거 절대 위치에서 트리거 발생, 절대 위치 방식

                //  dPos : 주기 선택시 : 위치마다위치마다 출력하기때문에 그 위치
                //         절대 선택시 : 출력할 그 위치, 이 위치와같으면 무조건 출력이 나간다. 
                //  주의사항: N404, N804의 경우에는 AxmTriggerSetAbsPeriod의 주기모드로 설정할경우 처음 그위치가 범위 안에 있으므로 
                //            트리거 출력이 한번 발생한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerSetAbsPeriod(int nAxisNo, uint uMethod, double dPos);

                // 지정 축에 트리거 기능의 사용 여부, 출력 레벨, 위치 비교기, 트리거 신호 지속 시간 및 트리거 출력 모드를 반환한다.
                // 주의사항: IP에서는 AxmTriiggerSetBlock함수를 호출시 내부라이브러리에서 설정값이 ABS_POS_MODE로 사용하기 때문에 
                // 이함수를 반환하는값이 1로 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerGetAbsPeriod(int nAxisNo, ref uint upMethod, ref double dpPos);

                //  사용자가 지정한 시작위치부터 종료위치까지 일정구간마다 트리거를 출력 한다.
                // 주의사항: SMC-2V03모듈 IP의 경우 트리거 시작 위치를 지나지 않으면 트리거 발생하지 않는다.
                //           SMC-2V03모듈 IP의 경우 트리거 종료 위치를 지나서 다시 트리거 범위안에 들어오면 트리거 발생하지않는다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerSetBlock(int nAxisNo, double dStartPos, double dEndPos, double dPeriodPos);
                // 'AxmTriggerSetBlock' 함수의 트리거 설정한 값을 읽는다..
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerGetBlock(int nAxisNo, ref double dpStartPos, ref double dpEndPos, ref double dpPeriodPos);
                // 사용자가 한 개의 트리거 펄스를 출력한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerOneShot(int nAxisNo);
                // 사용자가 한 개의 트리거 펄스를 몇초후에 출력한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerSetTimerOneshot(int nAxisNo, int mSec);
                // 절대위치 트리거 무한대 절대위치 출력한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerOnlyAbs(int nAxisNo, int nTrigNum, ref double dTrigPos);
                // 트리거 설정을 리셋한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmTriggerSetReset(int nAxisNo);

                //======== CRC( 잔여 펄스 클리어 함수)=====================================================================    

                //Level   : LOW(0), HIGH(1), UNUSED(2), USED(3)
                //uMethod : 잔여펄스 제거 출력 신호 펄스 폭 2 - 6까지 설정가능.(QI만 사용, IP사용안함)
                //          0 : Don't care , 1 : Don't care, 2: 500 uSec, 3:1 mSec, 4:10 mSec, 5:50 mSec, 6:100 mSec

                //지정 축에 CRC 신호 사용 여부 및 출력 레벨을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcSetMaskLevel(int nAxisNo, uint uLevel, uint uMethod);
                // 지정 축의 CRC 신호 사용 여부 및 출력 레벨을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcGetMaskLevel(int nAxisNo, ref uint upLevel, ref uint upMethod);

                //uOnOff  : CRC 신호를 Program으로 발생 여부  (FALSE(0),TRUE(1))

                // 지정 축에 CRC 신호를 강제로 발생 시킨다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcSetOutput(int nAxisNo, uint uOnOff);
                // 지정 축의 CRC 신호를 강제로 발생 여부를 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcGetOutput(int nAxisNo, ref uint upOnOff);

                //-----------    SMC-2V03 module 전용 함수 : EndLimit을 만날때 강제로 신호를 발생시킨다. --------
                // uPositiveUse : Positive Emeregency End limit에 대한 Clear출력 사용 레벨
                // uNegativeUse : Negative Emeregency End limit에 대한 Clear출력 사용 레벨
                // Level   : LOW(0), HIGH(1), UNUSED(2)
                // 지정 축에 리미트에 대한 CRC 신호의 사용 여부 및 출력 레벨을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcSetEndLimit(int nAxisNo, uint uPositiveLevel, uint uNegativeLevel);
                // 지정 축의 리미트에 대한 CRC 신호의 사용 여부 및 출력 레벨을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmCrcGetEndLimit(int nAxisNo, ref uint upPositiveLevel, ref uint upNegativeLevel);

                //======MPG(Manual Pulse Generation) 함수===========================================================
                //================ SMC-2V03 module ===========================================================
                // lInputMethod : 0-7 까지 설정가능. 0:OnePhase, 1:TwoPhase1, 2:TwoPhase2, 3:TwoPhase4
                //                                   4:Level One Phase, 5:Level Two Phase1, 6: Level Two Phase2, 7:Level Two Phase4
                // lDriveMode   : 0-2 까지 설정가능 (0 :MPG 슬레이브 모드 ,1 :MPG PRESET 모드, 2 :MPG 연속 모드)
                // MPGPos        : MPG 입력신호마다 이동하는 거리
                // dMPGdenominator, dMPGnumerator 사용안함.


                //================ PCI-Nx04 ============================================================
                // lInputMethod : 0-3 까지 설정가능. 0:OnePhase, 1:TwoPhase1(IP만가능, QI지원안함) , 2:TwoPhase2, 3:TwoPhase4
                // lDriveMode   : 0만 설정가능 (0 :MPG 연속모드)
                // MPGPos        : MPG 입력신호마다 이동하는 거리
                // MPGdenominator: MPG(수동 펄스 발생 장치 입력)구동 시 나누기 값
                // dMPGnumerator : MPG(수동 펄스 발생 장치 입력)구동 시 곱하기 값
                // dwNumerator   : 최대(1 에서    64) 까지 설정 가능
                // dwDenominator : 최대(1 에서  4096) 까지 설정 가능
                // dMPGdenominator = 4096, MPGnumerator=1 가 의미하는 것은 
                // MPG 한바퀴에 200펄스면 그대로 1:1로 1펄스씩 출력을 의미한다. 
                // 만약 dMPGdenominator = 4096, MPGnumerator=2 로 했을경우는 1:2로 2펄스씩 출력을 내보낸다는의미이다. 
                // 여기에 MPG PULSE = ((Numerator) * (Denominator)/ 4096 ) 칩내부에 출력나가는 계산식이다.


                // 지정 축에 MPG 입력방식, 드라이브 구동 모드, 이동 거리, MPG 속도 등을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMPGSetEnable(int nAxisNo, int nInputMethod, int nDriveMode, double dMPGPos, double dVel, double dAccel);
                // 지정 축에 MPG 입력방식, 드라이브 구동 모드, 이동 거리, MPG 속도 등을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMPGGetEnable(int nAxisNo, ref int npInputMethod, ref int npDriveMode, ref double dpMPGPos, ref double dpVel);

                // IP 사용안함, QI 전용 함수.
                // 지정 축에 MPG 드라이브 구동 모드에서 한펄스당 이동할 펄스 비율을 설정한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMPGSetRatio(int nAxisNo, double dMPGnumerator, double dMPGdenominator);
                // 지정 축에 MPG 드라이브 구동 모드에서 한펄스당 이동할 펄스 비율을 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMPGGetRatio(int nAxisNo, ref double dMPGnumerator, ref double dMPGdenominator);

                // 지정 축에 MPG 드라이브 설정을 해지한다.
                [DllImport("AXL.dll")]
                public static extern uint AxmMPGReset(int nAxisNo);

                //======= 헬리컬 이동  (PCI-Nx04 전용 함수)===========================================================================
                // 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 보간 구동하는 함수이다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 중심점을 지정하여 헬리컬 연속보간 구동하는 함수이다. 
                // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다)
                // dCenterPos = 중심점 X,Y  , dEndPos = 종료점 X,Y     
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향    
                [DllImport("AXL.dll")]
                public static extern uint AxmHelixCenterMove(int lCoord, double dCenterXPos, double dCenterYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir);
                // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다. 
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 중간점, 종료점을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
                // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
                // dMidPos = 중간점 X,Y  , dEndPos = 종료점 X,Y 
                [DllImport("AXL.dll")]
                public static extern uint AxmHelixPointMove(int lCoord, double dMidXPos, double dMidYPos, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel);
                // 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬 보간 구동하는 함수이다.
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 종료점과 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
                // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
                // dRadius = 반지름, dEndPos = 종료점 X,Y  , uShortDistance = 작은원(0), 큰원(1)
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향    
                [DllImport("AXL.dll")]
                public static extern uint AxmHelixRadiusMove(int lCoord, double dRadius, double dEndXPos, double dEndYPos, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir, uint uShortDistance);
                // 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬 보간 구동하는 함수이다
                // AxmContiBeginNode, AxmContiEndNode와 같이사용시 지정된 좌표계에 시작점, 회전각도와 반지름을 지정하여 헬리컬연속 보간 구동하는 함수이다. 
                // 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다. AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)
                //dCenterPos = 중심점 X,Y  , dAngle = 각도.
                // uCWDir   DIR_CCW(0): 반시계방향, DIR_CW(1) 시계방향    
                [DllImport("AXL.dll")]
                public static extern uint AxmHelixAngleMove(int lCoord, double dCenterXPos, double dCenterYPos, double dAngle, double dZPos, double dVel, double dAccel, double dDecel, uint uCWDir);

                //======== 스플라인 이동 (PCI-Nx04 전용 함수)=========================================================================== 

                // AxmContiBeginNode, AxmContiEndNode와 같이사용안함. 
                // 스플라인 연속 보간 구동하는 함수이다. 원호 연속 보간 구동을 위해 내부 Queue에 저장하는 함수이다.
                // AxmContiStart함수를 사용해서 시작한다. (연속보간 함수와 같이 이용한다.)    
                // lPosSize : 최소 3개 이상.
                // 2축으로 사용시 dPoZ값을 0으로 넣어주면 됨.
                // 3축으로 사용시 축맵핑을 3개및 dPosZ 값을 넣어준다.
                [DllImport("AXL.dll")]
                public static extern uint AxmSplineWrite(int lCoord, int lPosSize, ref double dPosX, ref double dPosY, double dVel, double dAccel, double dDecel, double dPosZ, int lPointFactor);

                //--------------------------------------------------------------------------------------------------------------------------------

            }

            public class CAXD
            {
                //========== 보드 및 모듈 정보 =================================================================================

                // DIO 모듈이 있는지 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoIsDIOModule(ref uint upStatus);

                // DIO 모듈 No 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetModuleNo(int lBoardNo, int lModulePos, ref int lpModuleNo);

                // DIO 입출력 모듈의 개수 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetModuleCount(ref int lpModuleCount);

                // 지정한 모듈의 입력 접점 개수 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetInputCount(int lModuleNo, ref int lpCount);

                // 지정한 모듈의 출력 접점 개수 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetOutputCount(int lModuleNo, ref int lpCount);

                // 지정한 모듈 번호로 베이스 보드 번호, 모듈 위치, 모듈 ID 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetModule(int lModuleNo, ref int lpBoardNo, ref int lpModulePos, ref uint upModuleID);

                // 해당 모듈이 제어가 가능한 상태인지 반환한다.
                [DllImport("AXL.dll")]
                public static extern uint AxdInfoGetModuleStatus(int lModuleNo);

                //========== 인터럽트 설정 확인 =================================================================================

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
                public static extern uint AxdiInterruptSetModule(int lModuleNo, IntPtr hWnd, uint uMessage, CAXHS.AXT_INTERRUPT_PROC pProc, ref uint pEvent);

                // 지정한 모듈의 인터럽트 사용 유무 설정
                //======================================================//
                // uUse        : DISABLE(0)    // 인터럽트 해제
                //             : ENABLE(1)     // 인터럽트 설정
                //======================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptSetModuleEnable(int lModuleNo, uint uUse);

                // 지정한 모듈의 인터럽트 사용 유무 확인
                //======================================================//
                // *upUse      : DISABLE(0)    // 인터럽트 해제
                //             : ENABLE(1)     // 인터럽트 설정
                //======================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptGetModuleEnable(int lModuleNo, ref uint upUse);

                // 인터럽트 발생 위치 확인
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptRead(ref int lpModuleNo, ref uint upFlag);

                //========== 인터럽트 상승 / 하강 에지 설정 확인 =================================================================================
                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // uValue       : DISABLE(0)
                //              : ENABLE(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeSetBit(int lModuleNo, int lOffset, uint uMode, uint uValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 byte 단위로 상승 또는 하강 에지 값을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // uValue       : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeSetByte(int lModuleNo, int lOffset, uint uMode, uint uValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 word 단위로 상승 또는 하강 에지 값을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // uValue       : 0x00 ~ 0x0FFFF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeSetWord(int lModuleNo, int lOffset, uint uMode, uint uValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 double word 단위로 상승 또는 하강 에지 값을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // uValue       : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeSetDword(int lModuleNo, int lOffset, uint uMode, uint uValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // *upValue     : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeGetBit(int lModuleNo, int lOffset, uint uMode, ref uint upValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 byte 단위로 상승 또는 하강 에지 값을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // *upValue     : 0x00 ~ 0x0FF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeGetByte(int lModuleNo, int lOffset, uint uMode, ref uint upValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 word 단위로 상승 또는 하강 에지 값을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // *upValue     : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeGetWord(int lModuleNo, int lOffset, uint uMode, ref uint upValue);

                // 지정한 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 double word 단위로 상승 또는 하강 에지 값을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // *upValue     : 0x00 ~ 0x0FFFFFFFF ('1'로 Setting 된 부분 인터럽트 설정)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeGetDword(int lModuleNo, int lOffset, uint uMode, ref uint upValue);

                // 전체 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위치에서 bit 단위로 상승 또는 하강 에지 값을 설정
                //===============================================================================================//
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // uValue       : DISABLE(0)
                //              : ENABLE(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeSet(int lOffset, uint uMode, uint uValue);

                // 전체 입력 접점 모듈, Interrupt Rising / Falling Edge register의 Offset 위정에서 bit 단위로 상승 또는 하강 에지 값을 확인
                //===============================================================================================//
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uMode        : DOWN_EDGE(0)
                //              : UP_EDGE(1)
                // *upValue     : DISABLE(0)
                //              : ENABLE(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiInterruptEdgeGet(int lOffset, uint uMode, ref uint upValue);

                //========== 입출력 레벨 설정 확인 =================================================================================
                //==입력 레벨 설정 확인
                // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelSetInportBit(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelSetInportByte(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelSetInportWord(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelSetInportDword(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelGetInportBit(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upLevel     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelGetInportByte(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upLevel     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelGetInportWord(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upLevel     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelGetInportDword(int lModuleNo, int lOffset, ref uint upLevel);

                // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lOffset      : 입력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelSetInport(int lOffset, uint uLevel);

                // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiLevelGetInport(int lOffset, ref uint upLevel);

                //==출력 레벨 설정 확인
                // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelSetOutportBit(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelSetOutportByte(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelSetOutportWord(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelSetOutportDword(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelGetOutportBit(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelGetOutportByte(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelGetOutportWord(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelGetOutportDword(int lModuleNo, int lOffset, ref uint upLevel);

                // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelSetOutport(int lOffset, uint uLevel);

                // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 확인
                //===============================================================================================//
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoLevelGetOutport(int lOffset, ref uint upLevel);

                //========== 입출력 포트 쓰기 읽기 =================================================================================
                //==출력 포트 쓰기
                // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
                //===============================================================================================//
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoWriteOutport(int lOffset, uint uValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 출력
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uLevel       : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoWriteOutportBit(int lModuleNo, int lOffset, uint uValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 출력
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uValue       : 0x00 ~ 0x0FF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoWriteOutportByte(int lModuleNo, int lOffset, uint uValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 출력
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uValue       : 0x00 ~ 0x0FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoWriteOutportWord(int lModuleNo, int lOffset, uint uValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 출력
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uValue       : 0x00 ~ 0x0FFFFFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoWriteOutportDword(int lModuleNo, int lOffset, uint uValue);

                //==출력 포트 읽기    
                // 전체 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoReadOutport(int lOffset, ref uint upValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upLevel     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoReadOutportBit(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoReadOutportByte(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoReadOutportWord(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 출력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoReadOutportDword(int lModuleNo, int lOffset, ref uint upValue);

                //==입력 포트 일기    
                // 전체 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiReadInport(int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : LOW(0)
                //              : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiReadInportBit(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiReadInportByte(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiReadInportWord(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 double word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : 0x00 ~ 0x0FFFFFFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiReadInportDword(int lModuleNo, int lOffset, ref uint upValue);

                //== MLII 용 M-Systems DIO(R7 series) 전용 함수.
                // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
                // *upValue    : LOW(0)
                //             : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtInportBit(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
                // *upValue    : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtInportByte(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // *upValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtInportWord(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 입력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // *upValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtInportDword(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0~15)
                // *upValue    : LOW(0)
                //             : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtOutportBit(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0~1)
                // *upValue    : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtOutportByte(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0)
                // *upValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtOutportWord(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터를 읽기
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0)
                // *upValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdReadExtOutportDword(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 출력
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치
                // uValue      : LOW(0)
                //             : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdWriteExtOutportBit(int lModuleNo, int lOffset, uint uValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 출력
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0~1)
                // uValue      : 0x00 ~ 0x0FF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdWriteExtOutportByte(int lModuleNo, int lOffset, uint uValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 출력
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0)
                // uValue    : 0x00 ~ 0x0FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdWriteExtOutportWord(int lModuleNo, int lOffset, uint uValue);

                // 지정한 모듈에 장착된 출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 출력
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 출력 접점에 대한 Offset 위치(0)
                // uValue    : 0x00 ~ 0x00000FFFF('1'로 읽힌 비트는 HIGH, '0'으로 읽힌 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdWriteExtOutportDword(int lModuleNo, int lOffset, uint uValue);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
                // uLevel      : LOW(0)
                //             : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelSetExtportBit(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
                // uLevel      : 0x00 ~ 0xFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelSetExtportByte(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // uLevel      : 0x00 ~ 0xFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelSetExtportWord(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 레벨을 설정
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // uLevel      : 0x00 ~ 0x0000FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelSetExtportDword(int lModuleNo, int lOffset, uint uLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 bit 단위로 데이터 레벨 확인
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~15)
                // *upLevel      : LOW(0)
                //             : HIGH(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelGetExtportBit(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 byte 단위로 데이터 레벨 확인
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0~1)
                // *upLevel      : 0x00 ~ 0xFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelGetExtportByte(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 word 단위로 데이터 레벨 확인
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // *upLevel      : 0x00 ~ 0xFFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelGetExtportWord(int lModuleNo, int lOffset, ref uint upLevel);

                // 지정한 모듈에 장착된 입/출력 접점용 확장 기능 모듈의 Offset 위치에서 dword 단위로 데이터 레벨 확인
                //===============================================================================================//
                // lModuleNo   : 모듈 번호
                // lOffset     : 입력 접점에 대한 Offset 위치(0)
                // *upLevel      : 0x00 ~ 0x0000FFFF('1'로 설정 된 비트는 HIGH, '0'으로 설정 된 비트는 LOW)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdLevelGetExtportDword(int lModuleNo, int lOffset, ref uint upLevel);

                //========== 고급 함수 =================================================================================
                // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 Off에서 On으로 바뀌었는지 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : FALSE(0)
                //              : TRUE(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiIsPulseOn(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 On에서 Off으로 바뀌었는지 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // *upValue     : FALSE(0)
                //              : TRUE(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiIsPulseOff(int lModuleNo, int lOffset, ref uint upValue);

                // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 count 만큼 호출될 동안 On 상태로 유지하는지 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 입력 접점에 대한 Offset 위치
                // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
                // *upValue     : FALSE(0)
                //              : TRUE(1)
                // lStart       : 1(최초 호출)
                //              : 0(반복 호출)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiIsOn(int lModuleNo, int lOffset, int lCount, ref uint upValue, int lStart);

                // 지정한 입력 접점 모듈의 Offset 위치에서 신호가 count 만큼 호출될 동안 Off 상태로 유지하는지 확인
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
                // *upValue     : FALSE(0)
                //              : TRUE(1)
                // lStart       : 1(최초 호출)
                //              : 0(반복 호출)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdiIsOff(int lModuleNo, int lOffset, int lCount, ref uint upValue, int lStart);

                // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 mSec동안 On을 유지하다가 Off 시킴
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
                // lmSec        : 1 ~ 30000
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoOutPulseOn(int lModuleNo, int lOffset, int lmSec);

                // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 mSec동안 Off를 유지하다가 On 시킴
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // lCount       : 0 ~ 0x7FFFFFFF(2147483647)
                // lmSec        : 1 ~ 30000
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoOutPulseOff(int lModuleNo, int lOffset, int lmSec);

                // 지정한 출력 접점 모듈의 Offset 위치에서 설정한 횟수, 설정한 간격으로 토글한 후 원래의 출력상태를 유지함
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // lInitState   : Off(0)
                //              : On(1)
                // lmSecOn      : 1 ~ 30000
                // lmSecOff     : 1 ~ 30000
                // lCount       : 1 ~ 0x7FFFFFFF(2147483647)
                //              : -1 무한 토글
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoToggleStart(int lModuleNo, int lOffset, int lInitState, int lmSecOn, int lmSecOff, int lCount);

                // 지정한 출력 접점 모듈의 Offset 위치에서 토글중인 출력을 설정한 신호 상태로 정지 시킴
                //===============================================================================================//
                // lModuleNo    : 모듈 번호
                // lOffset      : 출력 접점에 대한 Offset 위치
                // uOnOff       : Off(0)
                //              : On(1)
                //===============================================================================================//
                [DllImport("AXL.dll")]
                public static extern uint AxdoToggleStop(int lModuleNo, int lOffset, uint uOnOff);
            }
        }

        protected struct NodeInfo
        {
            public ushort ModuleID { get; set; }
            public ushort InputSize { get; set; }
            public ushort OutputSize { get; set; }
            public byte[] InputIO { get; set; }
            public byte[] OutputIO { get; set; }
        };

        private Dictionary<int, NodeInfo> _nodeInfoList = new Dictionary<int, NodeInfo>();

        protected Dictionary<int, NodeInfo> NodeInfoList { get { return _nodeInfoList; } }

        public override void Open()
        {
            //uint result = 0;
            //if (AjinLibrary.CAXL.AxlIsOpened() != (int)AjinLibrary.AXT_BOOLEAN.TRUE)
            //{
            //    result = AjinLibrary.CAXL.AxlOpenNoReset(0);

            //    if (result != (uint)AjinLibrary.AXT_FUNC_RESULT.AXT_RT_SUCCESS)
            //        throw new Exception(this.Name + " is initializing fail");
            //}
        }

        public override void Close()
        {
            //AjinLibrary.CAXL.AxlClose();
        }

        public override void LoadParameters(System.Xml.Linq.XElement xml)
        {
            base.LoadParameters(xml);

            try
            {
                if (xml.Element("NodeInfoList") != null)
                    LoadNodeInfo(xml.Element("NodeInfoList"));
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public override bool GetInputIOValue(int index)
        {
            try
            {
                int moduleID = index / 10000;
                int byteIndex = (index - moduleID * 10000) / 10;
                int bitIndex = index % 10;
                return Utility.FAUtility.CheckBit(NodeInfoList[moduleID].InputIO[byteIndex], bitIndex);
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
                int moduleID = (ushort)(index / 10000);
                int byteIndex = (index - moduleID * 10000) / 10;
                int bitIndex = index % 10;
                NodeInfoList[moduleID].InputIO[byteIndex] =
                    Utility.FAUtility.SetBit(NodeInfoList[moduleID].InputIO[byteIndex], bitIndex, value);
            }
            catch
            {
            }
        }

        public override bool GetOutputIOValue(int index)
        {
            try
            {
                int moduleID = index / 10000;
                int byteIndex = (index - moduleID * 10000) / 10;
                int bitIndex = index % 10;
                if (NodeInfoList.ContainsKey(moduleID) == false) return false;
                if (NodeInfoList[moduleID].OutputIO.Length <= byteIndex) return false;

                return Utility.FAUtility.CheckBit(NodeInfoList[moduleID].OutputIO[byteIndex], bitIndex);
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
                int moduleID = (ushort)(index / 10000);
                int byteIndex = (index - moduleID * 10000) / 10;
                int bitIndex = index % 10;
                NodeInfoList[moduleID].OutputIO[byteIndex] =
                    Utility.FAUtility.SetBit(NodeInfoList[moduleID].OutputIO[byteIndex], bitIndex, value);
            }
            catch
            {
            }
        }

        public override void GetInputIOBytes(int index, byte[] bytes)
        {
            int moduleID = index / 10000;
            int byteIndex = (index - moduleID * 10000) / 10;
            if (NodeInfoList.ContainsKey(moduleID) == false) return;
            if (NodeInfoList[moduleID].OutputIO.Length <= byteIndex) return;

            for (int i = 0; i < bytes.Length; i++)
            {
                int currentIndex = byteIndex + i;
                if (NodeInfoList[moduleID].OutputIO.Length <= currentIndex) break;
                bytes[i] = NodeInfoList[moduleID].OutputIO[currentIndex];
            }
        }

        public override void SetOutputIOBytes(int index, byte[] bytes)
        {
            try
            {
                int moduleID = (ushort)(index / 10000);
                int currentIndex = (index - moduleID * 10000) / 10;

                for (int i = 0; i < bytes.Length; i++)
                {
                    if (NodeInfoList[moduleID].OutputIO.Length <= currentIndex) break;
                    NodeInfoList[moduleID].OutputIO[currentIndex] = bytes[i];
                }
            }
            catch
            {
            }
        }

        public override void ReadWrite()
        {
            foreach (KeyValuePair<int, NodeInfo> nodeInfo in NodeInfoList)
            {
                for (int offset = 0; offset < nodeInfo.Value.InputIO.Count(); offset++)
                {
                    uint value = 0;
                    //AjinLibrary.CAXD.AxdiReadInportByte(nodeInfo.Value.ModuleID - 1, offset, ref value);
                    AjinLibrary.CAXM.AxmSignalReadInput(nodeInfo.Value.ModuleID - 1, ref value);

                    nodeInfo.Value.InputIO[offset] = (byte)value;
                }

                for (int offset = 0; offset < nodeInfo.Value.OutputIO.Count(); offset++)
                {
                    //uint value = nodeInfo.Value.OutputIO[offset];
                    uint value = nodeInfo.Value.OutputIO[offset];

                    //AjinLibrary.CAXD.AxdoWriteOutportByte(nodeInfo.Value.ModuleID - 1, offset, value);
                    AjinLibrary.CAXM.AxmSignalWriteOutput(nodeInfo.Value.ModuleID - 1, value);
                }
            }
        }

        private void LoadNodeInfo(XElement xml)
        {
            foreach (XElement item in xml.Elements())
            {
                NodeInfo nodeInfo = new NodeInfo();
                nodeInfo.ModuleID = ushort.Parse(item.Element("ModuleID").Value.Trim());
                nodeInfo.InputSize = ushort.Parse(item.Element("InputSize").Value.Trim());
                nodeInfo.OutputSize = ushort.Parse(item.Element("OutputSize").Value.Trim());
                nodeInfo.InputIO = new byte[nodeInfo.InputSize];
                nodeInfo.OutputIO = new byte[nodeInfo.OutputSize];
                NodeInfoList.Add(nodeInfo.ModuleID, nodeInfo);
            }
        }
    }
}
