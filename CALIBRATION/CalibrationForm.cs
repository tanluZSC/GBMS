using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

using GBMSAPI_NET;
using GBMSAPI_NET.GBMSAPI_NET_Defines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_AcquisitionProcessDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_DeviceCharacteristicsDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_ErrorCodesDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_RollFunctionalityDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLCDDefines;
using GBMSAPI_NET.GBMSAPI_NET_Defines.GBMSAPI_NET_VisualInterfaceLEDsDefines;
using GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions;

using GBMSAPI_CS_Example.UTILITY;

namespace GBMSAPI_CS_Example.CALIBRATION
{
    public partial class CalibrationForm : Form
    {
        protected IntPtr CalibrationImage;
		protected int CalibrationImageSX;
		protected int CalibrationImageSY;
		protected uint Diagnostic;
		// ver 2.10.0.0: use "2" functions
		//Boolean FlatScanArea;
		UInt32 ScanArea;
		// end ver 2.10.0.0: use "2" functions
		Image OriginalImage;

        public void ReleaseResources()
        {
           // in order to free allocated resources 
        }

        public CalibrationForm()
        {
            InitializeComponent();

			// ver 2.10.0.0: use "2" functions
			UInt32 scanA;
			int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetSupportedScanAreas(out scanA);
			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
			{
				GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal, "CalibrationForm,GBMSAPI_NET_GetSupportedScanAreas");
				return;
			}
			this.rbFullFrameArea.Enabled = false;
			this.rbRollIqsArea.Enabled = false;
			this.rbRollGaArea.Enabled = false;
			this.rbRolledThenarArea.Enabled = false;
			this.rbRolledJointArea.Enabled = false;
			if ((scanA & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_FULL_FRAME) != 0) this.rbFullFrameArea.Enabled = true;
			if ((scanA & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_IQS) != 0) this.rbRollIqsArea.Enabled = true;
			if ((scanA & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_GA) != 0) this.rbRollGaArea.Enabled = true;
			if ((scanA & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_THENAR) != 0) this.rbRolledThenarArea.Enabled = true;
			if ((scanA & GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_JOINT) != 0) this.rbRolledJointArea.Enabled = true;

			if (this.rbFullFrameArea.Enabled)
			{
				rbFullFrameArea.Checked = true;
				this.rbFullFrameArea_CheckedChanged(null, null);
			}
			else if (this.rbRollIqsArea.Enabled)
			{
				rbRollIqsArea.Checked = true;
				this.rbRollIqsArea_CheckedChanged(null, null);
			}
			else if (this.rbRollGaArea.Enabled)
			{
				rbRollGaArea.Checked = true;
				this.rbRollGaArea_CheckedChanged(null, null);
			}
			else if (this.rbRolledThenarArea.Enabled)
			{
				rbRolledThenarArea.Checked = true;
				this.rbRolledThenarArea_CheckedChanged(null, null);
			}
			else if (this.rbRolledJointArea.Enabled)
			{
				rbRolledJointArea.Checked = true;
				this.rbRolledJointArea_CheckedChanged(null, null);
			}
			else
			{
				MessageBox.Show("NO SCANNABLE AREAS AVAILABLE", "HEAVY ERROR!!!!");
				return;
			}
			// end ver 2.10.0.0: use "2" functions
			this.OriginalImage = this.CalibrationImagePictureBox.Image;
        }

		// ver 2.10.0.0: use "2" functions
		private String GetAreaNameFromCheckedRadioButton()
		{
			if (this.rbFullFrameArea.Enabled && this.rbFullFrameArea.Checked)
			{
				return "FULL_FRAME";
			}
			if (this.rbRolledJointArea.Enabled && this.rbRolledJointArea.Checked)
			{
				return "ROLL_JOINT";
			}
			if (this.rbRolledThenarArea.Enabled && this.rbRolledThenarArea.Checked)
			{
				return "ROLL_THENAR";
			}
			if (this.rbRollGaArea.Enabled && this.rbRollGaArea.Checked)
			{
				return "ROLL_GA";
			}
			if (this.rbRollIqsArea.Enabled && this.rbRollIqsArea.Checked)
			{
				return "ROLL_IQS";
			}
			return "";
		}
		private void SetScanArea(UInt32 scanA)
		{
			this.ScanArea = scanA;
			UInt32 ImSX, ImSY;
			int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetCalibrationImageSize2(
					this.ScanArea, out ImSX, out ImSY
				);

			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
			{
				GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
					"FlatAreaRadioButton_CheckedChanged,GBMSAPI_NET_GetCalibrationImageSize");
				return;
			}

			CalibrationImageSX = (int)ImSX;
			CalibrationImageSY = (int)ImSY;

			if (GBMSAPI_Example_Globals.CalibrationBuffer != IntPtr.Zero) Marshal.FreeHGlobal(GBMSAPI_Example_Globals.CalibrationBuffer);
			if (GBMSAPI_Example_Globals.DummyCalibrationBuffer != IntPtr.Zero)
				Marshal.FreeHGlobal(GBMSAPI_Example_Globals.DummyCalibrationBuffer);

			GBMSAPI_Example_Globals.CalibrationBuffer = Marshal.AllocHGlobal((int)(CalibrationImageSX * CalibrationImageSY));
			GBMSAPI_Example_Globals.DummyCalibrationBuffer = Marshal.AllocHGlobal((int)(CalibrationImageSX * CalibrationImageSY));
			if (GBMSAPI_Example_Globals.CalibrationBuffer == IntPtr.Zero ||
				GBMSAPI_Example_Globals.DummyCalibrationBuffer == IntPtr.Zero)
			{
				MessageBox.Show("CalibrationForm, SetScanArea: Memory allocation failed", "FATAL ERROR");
				return;
			}
			this.CalibrationImage = (GBMSAPI_Example_Globals.CalibrationBuffer);

			this.ImSXTextBox.Text = "" + CalibrationImageSX;
			this.ImSYTextBox.Text = "" + CalibrationImageSY;

			this.DiagnosticListBox.Items.Clear();
			this.CalibrationImagePictureBox.Image = this.OriginalImage;

			this.SaveButton.Enabled = false;
		}
		private void rbFullFrameArea_CheckedChanged(object sender, EventArgs e)
        {
			if (rbFullFrameArea.Checked == true)
			{
				// ver 2.10.0.0: use "2" functions
				//this.FlatScanArea = true;
				SetScanArea(GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_FULL_FRAME);
				// end ver 2.10.0.0: use "2" functions
			}
			else
			{
				return;
			}

			// ver 2.10.0.0: use "2" functions
			//int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetCalibrationImageSize(
			//        this.FlatScanArea, out ImSX, out ImSY
			//    );

			//if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
			//{
			//    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
			//        "FlatAreaRadioButton_CheckedChanged,GBMSAPI_NET_GetCalibrationImageSize");
			//    return;
			//}

			//CalibrationImageSX = (int)ImSX;
			//CalibrationImageSY = (int)ImSY;

			//this.ImSXTextBox.Text = "" + CalibrationImageSX;
			//this.ImSYTextBox.Text = "" + CalibrationImageSY;

			//this.DiagnosticListBox.Items.Clear();
			//this.CalibrationImagePictureBox.Image = this.OriginalImage;

			//this.SaveButton.Enabled = false;
			// ver 2.10.0.0: use "2" functions
		}


		private void rbRollIqsArea_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRollIqsArea.Checked == true)
			{
				SetScanArea(GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_IQS);
			}
			else
			{
				return;
			}
		}

		private void rbRollGaArea_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRollGaArea.Checked == true)
			{
				SetScanArea(GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_GA);
			}
			else
			{
				return;
			}
		}

		private void rbRolledJointArea_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRolledJointArea.Checked == true)
			{
				SetScanArea(GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_JOINT);
			}
			else
			{
				return;
			}
		}

		private void rbRolledThenarArea_CheckedChanged(object sender, EventArgs e)
		{
			if (rbRolledThenarArea.Checked == true)
			{
				SetScanArea(GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_ROLL_THENAR);
			}
			else
			{
				return;
			}
		}
		// end ver 2.10.0.0: use "2" functions
        private void GetNewCalibrationImageButton_Click(object sender, EventArgs e)
        {
            try
			{
				uint CalibrationDiagnostic;
                Byte [] CalImageArray;

				// ver 2.10.0.0: use "2" functions
				//int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetCalibrationImage(
				//    FlatScanArea,out CalImageArray,out CalibrationDiagnostic);
				int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_GetCalibrationImage2(
					this.ScanArea, out CalImageArray, out CalibrationDiagnostic);
				// end ver 2.10.0.0: use "2" functions

				if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                {
                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                        RetVal, "GetNewCalibrationImageButton_Click,GBMSAPI_GetImageSize");
                    return;
                }

                Marshal.Copy(CalImageArray, 0, this.CalibrationImage, CalImageArray.Length);

				// show diagnostic
				this.Diagnostic = CalibrationDiagnostic;
				this.DiagnosticListBox.Items.Clear();
				if (CalibrationDiagnostic != 0)
				{
                    List<String> DiagList = GBMSAPI_Example_Util.GetDiagStringsFromDiagMask(CalibrationDiagnostic);
                    if ((DiagList != null) && (DiagList.Count > 0))
                    {
                        for (int i = 0; i < DiagList.Count; i++)
                        {
                            this.DiagnosticListBox.Items.Add(DiagList[i]);
                        }
                    }
				}

                // be aware of stride
                int stride;
				IntPtr BmpBuffer;
				if ((CalibrationImageSX %4) != 0)
				{
					stride = CalibrationImageSX - (CalibrationImageSX %4);

                    Byte [] Dest; Byte [] Src;
                    Dest = new Byte[stride * CalibrationImageSY];
                    Src = new Byte[CalibrationImageSX * CalibrationImageSY];

                    if (Dest == null || Src == null)
                    {
                        MessageBox.Show("Error in memory allocation in GetNewCalibrationImageButton_Click");
                    }

                    Marshal.Copy(GBMSAPI_Example_Globals.CalibrationBuffer, Src, 0, Src.Length);
                    Marshal.Copy(GBMSAPI_Example_Globals.DummyCalibrationBuffer, Dest, 0, Dest.Length);

                    int DestOffset = 0, SrcOffset = 0;
                    for (int i = 0; i < CalibrationImageSY;
                        i ++, DestOffset += stride, SrcOffset += CalibrationImageSX)
                    {
                        Buffer.BlockCopy(Src,SrcOffset,Dest,DestOffset,stride);
                    }

                    Marshal.Copy(Dest, 0, GBMSAPI_Example_Globals.DummyCalibrationBuffer, Dest.Length);

                    BmpBuffer = GBMSAPI_Example_Globals.DummyCalibrationBuffer;
				}
				else
				{
					stride = CalibrationImageSX;
                    BmpBuffer = GBMSAPI_Example_Globals.CalibrationBuffer;
				}
				
				// draw image
				this.CalibrationImagePictureBox.Image = new Bitmap(
					CalibrationImageSX, CalibrationImageSY,
					stride,
                    System.Drawing.Imaging.PixelFormat.Format8bppIndexed,
					BmpBuffer
					);

                System.Drawing.Imaging.ColorPalette pal = this.CalibrationImagePictureBox.Image.Palette;
				for (int i = 0; i < pal.Entries.Length; i++)
				{
					pal.Entries[i] = Color.FromArgb(255, i, i, i);
				}
				this.CalibrationImagePictureBox.Image.Palette = pal;
				this.CalibrationImagePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;

				this.SaveButton.Enabled = true;

				this.Update();
			}
			catch(Exception ex)
			{
				MessageBox.Show("Exception in GetNewCalibrationImageButton_Click: " + ex.Message);
			}
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            /////////////////////
			// SAVE FILE
			/////////////////////
			Byte DeviceID;
            String DeviceSerialNumber;

			// Get Serial Number and ID
            
			int RetVal = GBMSAPI_NET_DeviceSettingRoutines.GBMSAPI_NET_GetCurrentDevice(
                out DeviceID,out DeviceSerialNumber);

			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "SaveButton_Click,GBMSAPI_GetImageSize");
                return;
            }

			// build file name
			// ver 2.10.0.0: scan area instead of flat/not flat
			//String FName = GBMSAPI_Example_Util.GBMSAPI_Example_GetDevNameFromDevID(DeviceID) + "_" +
			//    (DeviceSerialNumber) + "_" +
			//    ((this.FlatScanArea == true) ? ("FLAT") : ("ROLL")) + ".raw";
			String FName = GBMSAPI_Example_Util.GBMSAPI_Example_GetDevNameFromDevID(DeviceID) + "_" +
				(DeviceSerialNumber) + "_" +
				this.GetAreaNameFromCheckedRadioButton() + ".raw";
			// end ver 2.10.0.0: scan area instead of flat/not flat

			// build image array for saving
			Byte [] CalImageArray = new Byte[this.CalibrationImageSX * this.CalibrationImageSY];
			if (CalImageArray == null)
			{
                MessageBox.Show("MEMORY ALLOCATION ERROR in SaveButton_Click(): close application", "HEAVY ERROR");
				return;
			}

			Marshal.Copy(this.CalibrationImage,CalImageArray,0,CalImageArray.Length);

			// save image on file
			System.IO.FileStream FS = System.IO.File.Create(FName);
			if (FS != null)
			{
				FS.Write(CalImageArray,0,CalImageArray.Length);
				FS.Close();
				MessageBox.Show("Image saved in " + FName,"Calibration results");
			}
			else
			{
				MessageBox.Show("Cannot save file " + FName,"WARNING");
			}
        }

        private void SetFactoryCalibrationButton_Click(object sender, EventArgs e)
        {
            ///////////////////////////////
			// CALIBRATE DEVICE
			///////////////////////////////
			// ver 2.10.0.0: use "2" functions
			//int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_SetCalibrationImage(this.FlatScanArea, null);
			int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_SetCalibrationImage2(
				GBMSAPI_NET_ScanAreas.GBMSAPI_NET_SA_FULL_FRAME, null);
			// end ver 2.10.0.0: use "2" functions
			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "SetFactoryCalibrationButton_Click,GBMSAPI_GetImageSize");
                return;
            }

			MessageBox.Show("Device Calibrated","Calibration results");
        }

        private void CalibrateDeviceButton_Click(object sender, EventArgs e)
        {
            ////////////////////////////
			// OPEN IMAGE FILE
			////////////////////////////
			OpenFileDialog  OFDlg = new OpenFileDialog();
			OFDlg.Title = "Open Calibration Image";
			if (OFDlg.ShowDialog() != DialogResult.OK )
			{
				MessageBox.Show("Error in Opening File");
				return;
			}
			String fname = OFDlg.FileName;
			Byte []  CalImageBytes = System.IO.File.ReadAllBytes(fname);

			if (CalImageBytes.Length != (CalibrationImageSX) * (CalibrationImageSY))
			{
				MessageBox.Show("Error: file size not correct");
				return;
			}

			///////////////////////////////
			// CALIBRATE DEVICE
			///////////////////////////////
			// ver 2.10.0.0: use "2" functions
			//int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_SetCalibrationImage(
			//    this.FlatScanArea, CalImageBytes);
			int RetVal = GBMSAPI_NET_AuxiliaryRoutines.GBMSAPI_NET_SetCalibrationImage2(
				this.ScanArea, CalImageBytes);
			// end ver 2.10.0.0: use "2" functions
			if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
					RetVal, "CalibrateDeviceButton_Click,GBMSAPI_NET_SetCalibrationImage2");
            }
			else
			{
				MessageBox.Show("Device Calibrated","Calibration results");
			}

			return;
        }
	}
}