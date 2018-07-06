using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
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
using GBMSAPI_CS_Example.CALIBRATION;
using GBMSAPI_CS_Example.MAIN_FORM;

namespace GBMSAPI_CS_Example.ACQUISITION_FORM
{
    public partial class AcquisitionForm : Form
    {
        protected GBMASPI_CsExampleMainForm AcquisitionFormCallingWindow;
        protected Boolean DrawRollInfo;
        protected DateTime DelayBeforeStartTimer;
        protected Boolean FirstStart;

        public const UInt32 WAIT_START_STATUS = 0x00000001;
        public const UInt32 WAIT_SCANNER_STATUS = 0x00000002;
        public const UInt32 WAIT_FIRST_FRAME_STATUS = 0x00000004;
        public const UInt32 WAIT_PREVIEW_END_STATUS = 0x00000008;
        public const UInt32 WAIT_ACQUISITION_END_STATUS = 0x00000010;

        protected Image OriginalImage;

        public AcquisitionForm()
        {
            InitializeComponent();
            // Normally the other constructor should be used
            this.Close();
        }

        public AcquisitionForm(GBMASPI_CsExampleMainForm CallingWindow)
        {
            InitializeComponent();
            //
            //TODO: Add the constructor code here
            //
             GBMSAPI_Example_Globals.UseImageFinalization = true;
               this.cbExcludeFinalization.Checked = false;

            //////////////////////////////////
            // INIT GLOBALS
            //////////////////////////////////
            GBMSAPI_Example_Globals.InitGlobalAcqOptions();

            ///////////////////////////////////
            // SET CALLING WINDOW
            ///////////////////////////////////
            if (CallingWindow == null)
            {
                this.Close();
            }
            this.AcquisitionFormCallingWindow = CallingWindow;

            ///////////////////////////////////
            // load scan options
            ///////////////////////////////////
            Boolean res = this.AcquisitionFormCallingWindow.GetAcquisitionOptions(
                out GBMSAPI_Example_Globals.ObjToScan,
                out GBMSAPI_Example_Globals.OptionMask,
                out GBMSAPI_Example_Globals.DisplayOptionMask,
                out GBMSAPI_Example_Globals.ContrastLimitToDisplay,
                out GBMSAPI_Example_Globals.CompletenessLimitToDisplay,
                out GBMSAPI_Example_Globals.ClipRegionW,
                out GBMSAPI_Example_Globals.ClipRegionH,
                out GBMSAPI_Example_Globals.RollPreviewTimeout,
                out GBMSAPI_Example_Globals.ArtefactCleaningDepth,
                out GBMSAPI_Example_Globals.ContrastIntermediateLimitToDisplay,
                out GBMSAPI_Example_Globals.CompletenessIntermediateLimitToDisplay,
                out GBMSAPI_Example_Globals.IsRollAreaStandardGa,
                // ver 3.2.0.0
                out GBMSAPI_Example_Globals.RollBlockCompositionIsEnabled,
                // end ver 3.2.0.0
                // ver 4.0.0.0
                out GBMSAPI_Example_Globals.HwFfdThresholdToBeSet,
                out GBMSAPI_Example_Globals.SwFfdThresholdToBeSet
                // ver 4.0.0.0
                );

            if (res == false)
            {
                this.Close();
            }

            this.OriginalImage = this.AcquiredImagePictureBox.Image;

            ////////////////////////////////
            // SHOW OBJECT TO SCAN NAME
            ////////////////////////////////
            String ObjName = GBMSAPI_Example_Util.GetObjectToScanNameFromID(GBMSAPI_Example_Globals.ObjToScan);
            this.ObjectNameTextBox.Text = ObjName;

            //////////////////
            // ALLOC BITMAPS
            //////////////////

            // Get Type for GetImageSize(...) function
            uint ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(GBMSAPI_Example_Globals.ObjToScan);
            if (ObjType == GBMSAPI_NET_ScannableBiometricTypes.GBMSAPI_NET_SBT_NO_OBJECT_TYPE)
            {
                MessageBox.Show("Object to scan not recognized", "ERROR");
                this.Close();
            }

            GBMSAPI_Example_Globals.AcquisitionScanArea = GBMSAPI_Example_Util.GetScanAreaFromObjectType(
                ObjType,
                ((GBMSAPI_Example_Globals.OptionMask &
                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FLAT_SINGLE_FINGER_ON_ROLL_AREA) != 0
                ),
                GBMSAPI_Example_Globals.IsRollAreaStandardGa
            );
            // end ver 2.10.0.0: use Scan instead of Flat/Roll area
            int RetVal;
            // ver 3.1.0.0: use "3" function
            RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetImageSize3
                    (ObjType,
                    GBMSAPI_Example_Globals.OptionMask, GBMSAPI_Example_Globals.AcquisitionScanArea,
                    out GBMSAPI_Example_Globals.FullResImSX, out GBMSAPI_Example_Globals.FullResImSY,
                    out GBMSAPI_Example_Globals.PreviewImSX, out GBMSAPI_Example_Globals.PreviewImSY);
            // end ver 3.1.0.0: use "3" function
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR
            )
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "AcquisitionForm,GBMSAPI_NET_GetImageSize");
                return;
            }
            /****************************
            // ALLOC PREVIEW IMAGE BITMAP
            ****************************/
            // image allocation
            // ver 2.10.0.0: with stride
            GBMSAPI_Example_Globals.PreviewImage =
                new Bitmap((int)GBMSAPI_Example_Globals.PreviewImSX, (int)GBMSAPI_Example_Globals.PreviewImSY,
                    PixelFormat.Format8bppIndexed);
            if (GBMSAPI_Example_Globals.PreviewImage == null)
            {
                MessageBox.Show("AcquisitionForm Constructor: Memory allocation failed", "FATAL ERROR");
                this.Close();
            }
            // end ver 2.10.0.0: with stride
            // ver 3.1.0.0 size and location for paint box
            GBMSAPI_Example_Util.CalculatePaintBoxSizeAndLocation(GBMSAPI_Example_Globals.PreviewImSX, GBMSAPI_Example_Globals.PreviewImSY,
                GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalSize, GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalLocation,
                out GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForPreview, out GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForPreview);
            // end ver 3.1.0.0 size and location for paint box

            /*****************************
            // ALLOC FULL RES IMAGE BITMAP
            *****************************/
            // image allocation
            GBMSAPI_Example_Globals.FullResImage = new
                Bitmap((int)GBMSAPI_Example_Globals.FullResImSX, (int)GBMSAPI_Example_Globals.FullResImSY,
                PixelFormat.Format8bppIndexed);
            if (GBMSAPI_Example_Globals.FullResImage == null)
            {
                MessageBox.Show("AcquisitionForm Constructor: Memory allocation failed", "FATAL ERROR");
                this.Close();
            }
            // end ver 2.10.0.0
            // ver 3.1.0.0 size and location for paint box
            GBMSAPI_Example_Util.CalculatePaintBoxSizeAndLocation(GBMSAPI_Example_Globals.FullResImSX, GBMSAPI_Example_Globals.FullResImSY,
                GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalSize, GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalLocation,
                out GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForAcq, out GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForAcq);

            this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForPreview;
            this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForPreview;
            // end ver 3.1.0.0 size and location for paint box

            // ver 3.1.0.0
            /////////////////////////////////
            // UPDATE BG IMAGE BUTTON
            /////////////////////////////////
            UInt32 devFeatures = 0;
            RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(
                out devFeatures);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "AcquisitionForm,GBMSAPI_NET_GetDeviceFeatures");
                return;
            }
            if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_UPDATE_BACKGROUND_IMAGE) != 0 &&
                !(GBMSAPI_Example_Util.IsRolled(ObjType)) // flat objects only
                // ver 4.0.0.0
                && ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FULL_RES_PREVIEW) != 0) // full resolution images only
                // end ver 4.0.0.0
                )
            {
                this.bUpdateBackgroundImage.Enabled = true;
            }
            else
            {
                this.bUpdateBackgroundImage.Enabled = false;
            }
            // end ver 3.1.0.0

            /////////////////////////////////
            // INIT ACQUISITION STATE GLOBALS
            /////////////////////////////////
            GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_START_STATUS;
            GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_START_STATUS;
            //AcquisitionInProgress = false;
            // LastEventOccurred = 0;
            GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
            //FrameHasArrived = false;


            GBMSAPI_Example_Globals.LastFrameSizeX = 0;
            GBMSAPI_Example_Globals.LastFrameSizeY = 0;
            GBMSAPI_Example_Globals.LastFrameCurrentFrameRate = .0;
            GBMSAPI_Example_Globals.LastFrameNominalFrameRate = .0;

            GBMSAPI_Example_Globals.AutoCapturePhase = GBMSAPI_NET_AutoCapturePhase.GBMSAPI_NET_ACP_OFF;
            GBMSAPI_Example_Globals.ClippingRegionPosX = 0;
            GBMSAPI_Example_Globals.ClippingRegionPosY = 0;
            GBMSAPI_Example_Globals.ClippingRegionSizeX = 0;
            GBMSAPI_Example_Globals.ClippingRegionSizeY = 0;
            GBMSAPI_Example_Globals.RolledArtefactSize = 0;
            GBMSAPI_Example_Globals.MarkerFrame = null;
            GBMSAPI_Example_Globals.NotWipedArtefactFrame = null;
            GBMSAPI_Example_Globals.FlatFingerprintSize = 0;
            GBMSAPI_Example_Globals.ImageContrast = 0;
            GBMSAPI_Example_Globals.ImageSize = 0;
            GBMSAPI_Example_Globals.HalfLowerPalmCompleteness = 0;
            GBMSAPI_Example_Globals.SkipRequested = false;

            // ver 3.1.0.1
            GBMSAPI_Example_Globals.DryFingerPercent = GBMSAPI_Example_Globals.WetFingerPercent = 0;
            // ver 3.1.0.1

            DrawRollInfo = false;

            this.AutoCaptureAccettedLabel.BackColor = Color.White;
            this.FullResolutionPreviewAcceptedLabel.BackColor = Color.White;
            this.AcquireFlatObjectOnRollAreaAcceptedLabel.BackColor = Color.White;

            if (
                // VER 3.1.0.0: check all rolled objects
                (GBMSAPI_Example_Util.IsRolled(ObjType))
                // end VER 3.1.0.0: check all rolled objects
            )
            {
                if ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_NO_ROLL_PREVIEW) != 0)
                {
                    this.RollPreviewModeTextBox.Text = "NO PREVIEW";
                }
                else if ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0)
                {
                    this.RollPreviewModeTextBox.Text = "MANUAL";
                }
                else if ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ROLL_PREVIEW_TYPE) != 0)
                {
                    this.RollPreviewModeTextBox.Text = "CENTER (Auto)";
                }
                else
                {
                    this.RollPreviewModeTextBox.Text = "SIDE (Auto)";
                }
            }
            else
            {
                this.RollPreviewModeTextBox.Text = "";
            }

            // Start acquisition immediately
            this.DelayBeforeStartTimer = DateTime.Now;
            this.StartStopButton.Enabled = false;
            this.FirstStart = true;
            GBMSAPI_Example_Globals.ScannerStarted = false;
            GBMSAPI_Example_Globals.AcquisitionEnded = false;
            GBMSAPI_Example_Globals.BusyFrame = false;
            GBMSAPI_Example_Globals.PreviewEnded = false;



            /////////////////////
            // start timer
            /////////////////////
            this.AcquisitionManagementGeneralTimer.Start();
        }


        public void ReleaseResources()
        {
            GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
            this.AcquisitionManagementGeneralTimer.Stop();

            GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetLogoScreen();
        }

        private void StartStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                GBMSAPI_Example_Globals.AcquisitionButtonPressed = true;
            }
            catch (Exception ex)
            {
                GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                MessageBox.Show("Exception in StartStopButton_Click: " + ex.Message);
                this.Close();
            }
        }
        //关闭窗口，没有做图片保存
        private void AcceptImageButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //采集状态管理
        private void AcquisitionStateManagement()
        {
            try
            {
                switch (GBMSAPI_Example_Globals.AcquisitionStatus)
                {
                    case WAIT_START_STATUS:
                        {
                            if (GBMSAPI_Example_Globals.PreviousAcquisitionStatus != WAIT_START_STATUS)
                            {
                                // this.DiagnosticMessageListBox.Items.Add("WAIT_START_STATUS");
                            }
                            GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_START_STATUS;
                            this.PreviewLabel.Visible = false;
                            this.WaitFirstFrameLabel.Visible = false;
                            if (this.FirstStart == false &&
                                GBMSAPI_Example_Globals.AcquisitionButtonPressed == false)
                            {
                                this.StartStopButton.Text = "REPEAT";
                                this.StartStopButton.Enabled = true;
                            }
                            else
                            {
                                this.StartStopButton.Text = "START";
                            }

                            uint OptionalExternalEquipment;
                            int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(
                                out OptionalExternalEquipment);

                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                            {
                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                    RetVal, "AcquisitionStateManagement,GBMSAPI_NET_GetOptionalExternalEquipment");
                                OptionalExternalEquipment = 0;
                            }

                            // check pedal support
                            Boolean PedalState = false;
                            if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_PEDAL) != 0)
                            {
                                if (this.FirstStart == false)
                                {
                                    this.StartStopButton.Text += "(or press pedal)";
                                }
                                // check pedal state
                                GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_CheckPedalState(out PedalState);
                            }

                            Byte PressedButton = 0;
                            if ((OptionalExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                            {
                                RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_GetPressedButton(out PressedButton);
                                if (RetVal == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                {
                                    // if OK Button is pressed, close window
                                    if (PressedButton == GBMSAPI_NET_PressedButtonIDs.GBMSAPI_NET_VILCD_TOUCHSCREEN_OK_BUTTON)
                                    {
                                        this.Close();
                                        return;
                                    }
                                }


                            }

                            // check acquisition start request
                            if (this.FirstStart == false &&
                                GBMSAPI_Example_Globals.AcquisitionButtonPressed == true ||
                                PedalState == true ||
                                PressedButton == GBMSAPI_NET_PressedButtonIDs.GBMSAPI_NET_VILCD_TOUCHSCREEN_START_BUTTON || // from logo screen
                                PressedButton == GBMSAPI_NET_PressedButtonIDs.GBMSAPI_NET_VILCD_TOUCHSCREEN_REPEAT_BUTTON // from stop screen
                                )
                            {
                                // Clear Image
                                // ver 3.1.0.0 size and location for paint box
                                this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalLocation;
                                this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxOriginalSize;
                                // end ver 3.1.0.0 size and location for paint box
                                this.AcquiredImagePictureBox.Image = this.OriginalImage;
                                this.AcquiredImagePictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
                                GBMSAPI_Example_Globals.FrameNumber = 0;
                                GBMSAPI_Example_Globals.PaintNumber = 0;

                                // Reset variables
                                // LastEventOccurred = 0;
                                GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
                                GBMSAPI_Example_Globals.BusyFrame = false;

                                GBMSAPI_Example_Globals.LastFrameSizeX = 0;
                                GBMSAPI_Example_Globals.LastFrameSizeY = 0;
                                GBMSAPI_Example_Globals.LastFrameCurrentFrameRate = .0;
                                GBMSAPI_Example_Globals.LastFrameNominalFrameRate = .0;

                                GBMSAPI_Example_Globals.ClippingRegionPosX = 0;
                                GBMSAPI_Example_Globals.ClippingRegionPosY = 0;
                                GBMSAPI_Example_Globals.ClippingRegionSizeX = 0;
                                GBMSAPI_Example_Globals.ClippingRegionSizeY = 0;

                                GBMSAPI_Example_Globals.AutoCapturePhase = GBMSAPI_NET_AutoCapturePhase.GBMSAPI_NET_ACP_OFF;

                                GBMSAPI_Example_Globals.RolledArtefactSize = 0;
                                GBMSAPI_Example_Globals.MarkerFrame = null;
                                GBMSAPI_Example_Globals.NotWipedArtefactFrame = null;
                                GBMSAPI_Example_Globals.FlatFingerprintSize = 0;
                                GBMSAPI_Example_Globals.ImageContrast = 0;
                                GBMSAPI_Example_Globals.ImageSize = 0;
                                GBMSAPI_Example_Globals.HalfLowerPalmCompleteness = 0;

                                // ver 3.1.0.1
                                GBMSAPI_Example_Globals.DryFingerPercent = GBMSAPI_Example_Globals.WetFingerPercent = 0;
                                // ver 3.1.0.1

                                // Reset State conditions
                                GBMSAPI_Example_Globals.ScannerStarted = false;
                                GBMSAPI_Example_Globals.AcquisitionEnded = false;
                                GBMSAPI_Example_Globals.SkipRequested = false;
                                GBMSAPI_Example_Globals.PreviewEnded = false;

                                // Clear text boxes
                                this.StopTypeTextBox.Text = "";
                                this.ImSXTextBox.Text = "";
                                this.ImSYTextBox.Text = "";
                                this.AutoCapturePhaseTextBox.Text = "";
                                this.ContrastTextBox.Text = "";
                                this.SizeTextBox.Text = "";
                                this.HLPCompletenessTextBox.Text = "";

                                // clear Accepted options labels
                                this.AutoCaptureAccettedLabel.BackColor = Color.White;
                                this.FullResolutionPreviewAcceptedLabel.BackColor = Color.White;
                                this.AcquireFlatObjectOnRollAreaAcceptedLabel.BackColor = Color.White;

                                DrawRollInfo = false;

                                // ver 4.0.0.0: fake fingerprint
                                uint devFeatures = 0;
                                GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out devFeatures);
                                if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HW_ANTIFAKE) != 0)
                                {
                                    if (GBMSAPI_Example_Globals.HwFfdThresholdToBeSet >= 0)
                                    {
                                        RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetHardwareFakeFingerDetectionThreshold(GBMSAPI_Example_Globals.HwFfdThresholdToBeSet);
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
                                                "GBMSAPI_NET_SetHardwareFakeFingerDetectionThreshold in WAIT_START_STATUS");
                                            GBMSAPI_Example_Globals.HwFfdThresholdToBeSet = 100;
                                            return;
                                        }
                                    }
                                    GBMSAPI_Example_Globals.HwFfdFlag = false;
                                    GBMSAPI_Example_Globals.HwFfdDiagnosticValue = 0;
                                }
                                if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_SW_ANTIFAKE) != 0)
                                {
                                    if (GBMSAPI_Example_Globals.SwFfdThresholdToBeSet >= 0)
                                    {
                                        RetVal = GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_SetSoftwareFakeFingerDetectionThreshold(GBMSAPI_Example_Globals.SwFfdThresholdToBeSet);
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
                                                "GBMSAPI_NET_SetSoftwareFakeFingerDetectionThreshold in WAIT_START_STATUS");
                                            GBMSAPI_Example_Globals.SwFfdThresholdToBeSet = 50;
                                            return;
                                        }
                                    }
                                    GBMSAPI_Example_Globals.SwFfdFlag = false;
                                }
                                // end ver 4.0.0.0: fake fingerprint

                                ////////////////////////////////////
                                // START ACQUISITION 
                                ////////////////////////////////////
                                // ver 2.10.0.0: use "2" function, comment next lines
                                //RetVal = GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StartAcquisition(
                                //    GBMSAPI_Example_Globals.ObjToScan,
                                //    GBMSAPI_Example_Globals.OptionMask,
                                //    this.AcquisitionCallback,
                                //    IntPtr.Zero,
                                //    GBMSAPI_Example_Globals.DisplayOptionMask,
                                //    GBMSAPI_Example_Globals.ContrastLimitToDisplay,
                                //    GBMSAPI_Example_Globals.CompletenessLimitToDisplay
                                //    );
                                // end ver 2.10.0.0: use "2" function, comment next lines
                                // ver 2.10.0.0: use "2" function, add next lines
                                this.DiagnosticMessageListBox.Items.Clear();
                                // ver 3.2.0.0
                                if (GBMSAPI_Example_Globals.RollBlockCompositionIsEnabled)
                                {
                                    GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableBlockComposition(true);
                                }
                                else
                                {
                                    GBMSAPI_NET_ScanSettingsRoutines.GBMSAPI_NET_ROLL_EnableBlockComposition(false);
                                }
                                // end ver 3.2.0.0

                                // ver 3.3.0.0
                                GBMSAPI_Example_Globals.AcquisitionStartTime = DateTime.Now;
                                GBMSAPI_Example_Globals.FirstFrameArrived = false;
                                // end ver 3.3.0.0

                                RetVal = GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StartAcquisition2(
                                    GBMSAPI_Example_Globals.ObjToScan,
                                    GBMSAPI_Example_Globals.OptionMask,
                                    GBMSAPI_Example_Globals.AcquisitionScanArea,
                                    this.AcquisitionCallback,
                                    IntPtr.Zero,
                                    GBMSAPI_Example_Globals.DisplayOptionMask,
                                    GBMSAPI_Example_Globals.ContrastLimitToDisplay,
                                    GBMSAPI_Example_Globals.CompletenessLimitToDisplay
                                    );
                                // end ver 2.10.0.0: use "2" function, add next lines
                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                {
                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(RetVal,
                                        "GBMSAPI_NET_StartAcquisition in WAIT_START_STATUS");

                                }
                                else
                                {
                                    // disable Start button until scanner effectively started
                                    this.StartStopButton.Enabled = false;
                                    GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_SCANNER_STATUS;
                                }
                                GBMSAPI_Example_Globals.AcquisitionButtonPressed = false;

                            }

                            this.Update();
                            break;
                        }
                    case WAIT_SCANNER_STATUS:
                        {
                            if (GBMSAPI_Example_Globals.PreviousAcquisitionStatus != WAIT_SCANNER_STATUS)
                            {
                                this.DiagnosticMessageListBox.Items.Clear();
                                // this.DiagnosticMessageListBox.Items.Add("WAIT_SCANNER_STATUS");
                            }
                            GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_SCANNER_STATUS;
                            this.WaitFirstFrameLabel.Visible = true;
                            this.StartStopButton.Enabled = false;
                            this.PreviewLabel.Visible = false;

                            // check errors
                            if (GBMSAPI_Example_Globals.LastErrorCode != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR ||
                                GBMSAPI_Example_Globals.AcquisitionEnded == true)
                            {
                                // message to be shown: in case of error, it will be set to 0
                                Byte MessageToShowOnStopScreen =
                                    GBMSAPI_NET_PreloadedGeneralMessages.GBMSAPI_NET_VILCD_MSG_ACQUISITION_SUCCESSFUL;
                                // return to WAIT_START
                                if (GBMSAPI_Example_Globals.LastErrorCode !=
                                    GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                {
                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                        GBMSAPI_Example_Globals.LastErrorCode, "WAIT_SCANNER_STATUS");
                                    MessageToShowOnStopScreen = 0;
                                }
                                GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
                                GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_START_STATUS;
                                if (GBMSAPI_Example_Globals.AcquisitionEnded == true)
                                {
                                    // check stop type
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_STOP_TYPE) == 0)
                                    {
                                        // manual
                                        this.StopTypeTextBox.Text = "MANUAL STOP";
                                        UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                            GBMSAPI_Example_Globals.ObjToScan);
                                        if (GBMSAPI_Example_Util.IsRolled(ObjType))
                                        {
                                            if ((GBMSAPI_Example_Globals.OptionMask &
                                                GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) == 0)
                                                ShowRotationNotCompletedMessage();
                                        }
                                        else if
                                            (
                                                ((GBMSAPI_Example_Globals.OptionMask &
                                                GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                                            )
                                        {
                                            MessageBox.Show("Image capture manually stopped before automatic stop: check image quality",
                                                "WARNING");
                                        }
                                    }
                                    else
                                    {
                                        this.StopTypeTextBox.Text = "AUTO STOP";
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_TIMEOUT) != 0)
                                        {
                                            MessageBox.Show("ACQUISITION TIMEOUT HAS EXPIRED", "WARNING");
                                        }
                                    }

                                    // If LCD supported, enable OK button on stop screen
                                    uint ExternalEquipment;
                                    int RetVal =
                                        GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(
                                        out ExternalEquipment);

                                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                    {
                                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                            RetVal, "WAIT_SCANNER_STATUS");
                                        ExternalEquipment = 0;
                                    }

                                    if ((ExternalEquipment &
                                        GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                    {
                                        if (
                                            (GBMSAPI_Example_Globals.DisplayOptionMask &
                                            GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN) != 0
                                            // stop screen enabled after acquisition
                                        )
                                        {
                                            // enable OK button
                                            RetVal =
                                                GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableOKButtonOnStopScreen();
                                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                // VER 1100: commented next row and added the row after. Incorrect status shown
                                                //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                //    RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    RetVal, "WAIT_SCANNER_STATUS");
                                                // end VER 1100
                                            }
                                            // show message
                                            if (MessageToShowOnStopScreen != 0)
                                            {
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetPreloadedMessageOnStopScreen(
                                                    MessageToShowOnStopScreen,
                                                    GBMSAPI_NET_PreloadedMessagesArea.GBMSAPI_NET_VILCD_MSG_AREA);
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    // VER 1100: commented next row and added the row after. Incorrect status shown
                                                    //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    //    RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_SCANNER_STATUS");
                                                    // end VER 1100
                                                }
                                            }
                                            else
                                            {
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetCustomerMessageOnStopScreen(
                                                    "ERROR IN ACQUISITION",
                                                    GBMSAPI_NET_CustomMessagesDefines.GBMSAPI_NET_VILCD_CUSTOM_MSG_TYPE_ERROR_MSG);
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    // VER 1100: commented next row and added the row after. Incorrect status shown
                                                    //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    //    RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_SCANNER_STATUS");
                                                    // end VER 1100
                                                }
                                            }
                                        }
                                        else
                                        {
                                            RetVal =
                                                GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableStartButtonOnLogoScreen();
                                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                // VER 1100: commented next row and added the row after. Incorrect status shown
                                                //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                //    RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    RetVal, "WAIT_SCANNER_STATUS");
                                                // end VER 1100
                                            }
                                        }
                                    }

                                    GBMSAPI_Example_Globals.AcquisitionEnded = false;
                                }

                            }
                            else
                            {
                                if (GBMSAPI_Example_Globals.ScannerStarted == true)
                                {
                                    // update accepted options labels
                                    // check ignored options						
                                    if ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                                    {
                                        this.AutoCaptureAccettedLabel.BackColor = Color.Green;
                                        this.AutoCaptureAccettedLabel.Update();
                                    }

                                    if ((GBMSAPI_Example_Globals.OptionMask &
                                        GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_FLAT_SINGLE_FINGER_ON_ROLL_AREA) != 0)
                                    {
                                        this.AcquireFlatObjectOnRollAreaAcceptedLabel.BackColor = Color.Green;
                                    }

                                    // enable start button
                                    this.StartStopButton.Text = "STOP";
                                    this.StartStopButton.Enabled = true;
                                    this.StartStopButton.Update();

                                    // switch to next state
                                    GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_FIRST_FRAME_STATUS;
                                }
                            }

                            this.Update();
                            break;
                        }

                    case WAIT_FIRST_FRAME_STATUS:
                        {
                            if (GBMSAPI_Example_Globals.PreviousAcquisitionStatus != WAIT_FIRST_FRAME_STATUS)
                            {
                                this.DiagnosticMessageListBox.Items.Clear();
                                // this.DiagnosticMessageListBox.Items.Add("WAIT_FIRST_FRAME_STATUS");
                            }
                            GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_FIRST_FRAME_STATUS;
                            this.WaitFirstFrameLabel.Visible = true;
                            this.PreviewLabel.Visible = false;

                            // check errors
                            if (GBMSAPI_Example_Globals.LastErrorCode != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR ||
                                GBMSAPI_Example_Globals.AcquisitionEnded == true)
                            {
                                // message to be shown: in case of error, it will be set to 0
                                Byte MessageToShowOnStopScreen = GBMSAPI_NET_PreloadedGeneralMessages.GBMSAPI_NET_VILCD_MSG_ACQUISITION_SUCCESSFUL;
                                // return to WAIT_START
                                if (GBMSAPI_Example_Globals.LastErrorCode != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                {
                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                        GBMSAPI_Example_Globals.LastErrorCode, "WAIT_FIRST_FRAME_STATUS");
                                    MessageToShowOnStopScreen = 0;
                                }
                                GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
                                // return to WAIT_START
                                GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_START_STATUS;
                                if (GBMSAPI_Example_Globals.AcquisitionEnded == true)
                                {
                                    // check stop type
                                    if ((GBMSAPI_Example_Globals.LastEventInfo & GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_STOP_TYPE) == 0)
                                    {
                                        // manual
                                        this.StopTypeTextBox.Text = "MANUAL STOP";
                                        UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                             GBMSAPI_Example_Globals.ObjToScan);
                                        if (GBMSAPI_Example_Util.IsRolled(ObjType))
                                        {
                                            if ((GBMSAPI_Example_Globals.OptionMask &
                                                GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) == 0)
                                                ShowRotationNotCompletedMessage();
                                        }
                                        else if
                                            (
                                                ((GBMSAPI_Example_Globals.OptionMask &
                                                GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                                            )
                                        {
                                            MessageBox.Show("Image capture manually stopped before automatic stop: check image quality",
                                                "WARNING");
                                        }
                                    }
                                    else
                                    {
                                        this.StopTypeTextBox.Text = "AUTO STOP";
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_TIMEOUT) != 0)
                                        {
                                            MessageBox.Show("ACQUISITION TIMEOUT HAS EXPIRED", "WARNING");
                                        }
                                    }

                                    // If LCD supported, enable OK button on stop screen
                                    uint ExternalEquipment;
                                    int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(
                                        out ExternalEquipment);

                                    if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                    {
                                        // VER 1100: commented next row and added the row after. Incorrect status shown
                                        //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                        //RetVal, "WAIT_SCANNER_STATUS");
                                        GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                            RetVal, "WAIT_FIRST_FRAME_STATUS");
                                        // end VER 1100

                                        ExternalEquipment = 0;
                                    }
                                    if ((ExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                    {
                                        if ((GBMSAPI_Example_Globals.DisplayOptionMask &
                                            GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN) != 0
                                            // stop screen enabled after acquisition
                                        )
                                        {
                                            RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableOKButtonOnStopScreen();
                                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                // VER 1100: commented next row and added the row after. Incorrect status shown
                                                //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                //RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    RetVal, "WAIT_FIRST_FRAME_STATUS");
                                                // end VER 1100
                                            }
                                            // show message
                                            if (MessageToShowOnStopScreen != 0)
                                            {
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetPreloadedMessageOnStopScreen(
                                                    MessageToShowOnStopScreen,
                                                    GBMSAPI_NET_PreloadedMessagesArea.GBMSAPI_NET_VILCD_MSG_AREA);
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    // VER 1100: commented next row and added the row after. Incorrect status shown
                                                    //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    //RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_FIRST_FRAME_STATUS");
                                                    // end VER 1100
                                                }
                                            }
                                            else
                                            {
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetCustomerMessageOnStopScreen(
                                                    "ERROR IN ACQUISITION",
                                                    GBMSAPI_NET_CustomMessagesDefines.GBMSAPI_NET_VILCD_CUSTOM_MSG_TYPE_ERROR_MSG);
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    // VER 1100: commented next row and added the row after. Incorrect status shown
                                                    //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    //RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_FIRST_FRAME_STATUS");
                                                    // end VER 1100
                                                }
                                            }
                                        }
                                        else
                                        {
                                            RetVal =
                                                GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableStartButtonOnLogoScreen();
                                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                // VER 1100: commented next row and added the row after. Incorrect status shown
                                                //GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                //RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                    RetVal, "WAIT_FIRST_FRAME_STATUS");
                                                // end VER 1100
                                            }
                                        }
                                    }

                                    GBMSAPI_Example_Globals.AcquisitionEnded = false;
                                }
                            }
                            else if (GBMSAPI_Example_Globals.AcquisitionButtonPressed == true)
                            {
                                UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                    GBMSAPI_Example_Globals.ObjToScan);

                                if ((GBMSAPI_Example_Globals.OptionMask &
                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0 &&
                                    // VER 3.1.0.0: check all rolled objects
                                    (GBMSAPI_Example_Util.IsRolled(ObjType))
                                    // end VER 3.1.0.0: check all rolled objects
                                )
                                {
                                    int RetStopPreview = GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_ROLL_StopPreview();
                                    if (RetStopPreview != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                    {
                                        GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                    }
                                }
                                else
                                {
                                    GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                }
                                this.StartStopButton.Enabled = false;
                                GBMSAPI_Example_Globals.AcquisitionButtonPressed = false;
                            }
                            else
                            {
                                if (GBMSAPI_Example_Globals.BusyFrame == true)
                                {
                                    UInt32 NextState;
                                    // a valid frame has been acquired
                                    // visualize frame
                                    // check acquisition phase
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForPreview;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForPreview;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.PreviewImage;
                                        this.PreviewLabel.Visible = true;
                                        NextState = WAIT_PREVIEW_END_STATUS;
                                    }
                                    else
                                    {
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForAcq;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForAcq;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.FullResImage;
                                        this.PreviewLabel.Visible = false;
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                    }
                                    // check image resolution
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_PREVIEW_RES) == 0)
                                        {
                                            this.ImageResTextBox.Text = "LOW PREVIEW RES";
                                        }
                                        else
                                        {
                                            this.ImageResTextBox.Text = "FULL PREVIEW RES";
                                        }
                                    }
                                    else
                                    {
                                        this.ImageResTextBox.Text = "NA";
                                    }

                                    // show diagnostic
                                    this.DiagnosticMessageListBox.Items.Clear();
                                    if (GBMSAPI_Example_Globals.LastDiagnosticValue != 0)
                                    {
                                        List<String> DiagList = GBMSAPI_Example_Util.GetDiagStringsFromDiagMask(
                                            GBMSAPI_Example_Globals.LastDiagnosticValue);
                                        if ((DiagList != null) && (DiagList.Count > 0))
                                        {
                                            for (int i = 0; i < DiagList.Count; i++)
                                            {
                                                this.DiagnosticMessageListBox.Items.Add(DiagList[i]);
                                            }
                                        }
                                    }

                                    this.ImSXTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeX;
                                    this.ImSYTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeY;
                                    this.CurrentFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameCurrentFrameRate.ToString("F2");
                                    this.NominalFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameNominalFrameRate.ToString("F2");
                                    this.AutoCapturePhaseTextBox.Text =
                                        GBMSAPI_Example_Util.GetAutoCapturePhaseStringFromValue(GBMSAPI_Example_Globals.AutoCapturePhase);
                                    if (GBMSAPI_Example_Globals.AutoCapturePhase == GBMSAPI_NET_AutoCapturePhase.GBMSAPI_NET_ACP_BEST_IMAGE_STOP)
                                    {
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                        this.StartStopButton.Enabled = false;
                                    }
                                    this.ContrastTextBox.Text = "" + GBMSAPI_Example_Globals.ImageContrast;
                                    this.SizeTextBox.Text = "" + GBMSAPI_Example_Globals.ImageSize;
                                    this.HLPCompletenessTextBox.Text = "" + GBMSAPI_Example_Globals.HalfLowerPalmCompleteness;

                                    // ver 3.1.0.1
                                    this.tbDryAreaPercent.Text = "" + GBMSAPI_Example_Globals.DryFingerPercent;
                                    this.tbWetAreaPercent.Text = "" + GBMSAPI_Example_Globals.WetFingerPercent;
                                    // ver 3.1.0.1

                                    this.Refresh();
                                    GBMSAPI_Example_Globals.BusyFrame = false;

                                    // switch to next state
                                    GBMSAPI_Example_Globals.AcquisitionStatus = NextState;
                                }
                            }

                            this.Update();
                            break;
                        }

                    case WAIT_PREVIEW_END_STATUS:
                        {
                            if (GBMSAPI_Example_Globals.PreviousAcquisitionStatus != WAIT_PREVIEW_END_STATUS)
                            {
                                this.DiagnosticMessageListBox.Items.Clear();
                                // this.DiagnosticMessageListBox.Items.Add("WAIT_PREVIEW_END_STATUS");
                                GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_PREVIEW_END_STATUS;
                                this.WaitFirstFrameLabel.Visible = false;
                                this.PreviewLabel.Visible = true;
                                this.StartStopButton.Text = "Stop Preview";
                            }

                            uint ExternalEquipment;
                            int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(
                                out ExternalEquipment);

                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                            {
                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                    RetVal, "WAIT_PREVIEW_END_STATUS");
                                ExternalEquipment = 0;
                            }

                            // check errors
                            if (GBMSAPI_Example_Globals.LastErrorCode != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                            {
                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                    GBMSAPI_Example_Globals.LastErrorCode, "WAIT_PREVIEW_END_STATUS");
                                // return to WAIT_START
                                GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
                                GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_START_STATUS;
                                if ((ExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                {
                                    if ((GBMSAPI_Example_Globals.DisplayOptionMask & GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN) != 0
                                        // stop screen enabled after acquisition
                                    )
                                    {
                                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableOKButtonOnStopScreen();
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_PREVIEW_END_STATUS");
                                        }
                                        // show message
                                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetCustomerMessageOnStopScreen(
                                            "ERROR IN ACQUISITION",
                                            GBMSAPI_NET_CustomMessagesDefines.GBMSAPI_NET_VILCD_CUSTOM_MSG_TYPE_ERROR_MSG);
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_PREVIEW_END_STATUS");
                                        }
                                    }
                                    else
                                    {
                                        RetVal =
                                            GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableStartButtonOnLogoScreen();
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_PREVIEW_END_STATUS");
                                        }
                                    }
                                }
                            }
                            else if (GBMSAPI_Example_Globals.AcquisitionButtonPressed == true)
                            {
                                UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                    GBMSAPI_Example_Globals.ObjToScan);

                                if ((GBMSAPI_Example_Globals.OptionMask &
                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0 &&
                                    // VER 3.1.0.0: check all rolled objects
                                    (GBMSAPI_Example_Util.IsRolled(ObjType))
                                    // end VER 3.1.0.0: check all rolled objects
                                )
                                {
                                    int RetStopPreview = GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_ROLL_StopPreview();
                                    if (RetStopPreview != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                    {
                                        GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                        GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_ACQUISITION_END_STATUS;
                                        this.StartStopButton.Enabled = false;
                                    }
                                }
                                else
                                {
                                    GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                    GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_ACQUISITION_END_STATUS;
                                    this.StartStopButton.Enabled = false;
                                }
                                // this.DiagnosticMessageListBox.Items.Add("AcquisitionButtonPressed");

                                GBMSAPI_Example_Globals.AcquisitionButtonPressed = false;
                            }
                            else
                            {
                                if (GBMSAPI_Example_Globals.BusyFrame == true)
                                {
                                    UInt32 NextState;
                                    // a valid frame has been acquired
                                    // visualize frame
                                    // check acquisition phase
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForPreview;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForPreview;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.PreviewImage;
                                        this.PreviewLabel.Visible = true;
                                        NextState = WAIT_PREVIEW_END_STATUS;
                                    }
                                    else
                                    {
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForAcq;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForAcq;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.FullResImage;
                                        this.PreviewLabel.Visible = false;
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                    }
                                    // check image resolution
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_PREVIEW_RES) == 0)
                                        {
                                            this.ImageResTextBox.Text = "LOW PREVIEW RES";
                                        }
                                        else
                                        {
                                            this.ImageResTextBox.Text = "FULL PREVIEW RES";
                                        }
                                    }
                                    else
                                    {
                                        this.ImageResTextBox.Text = "NA";
                                    }

                                    // show diagnostic
                                    this.DiagnosticMessageListBox.Items.Clear();
                                    if (GBMSAPI_Example_Globals.LastDiagnosticValue != 0)
                                    {
                                        List<String> DiagList = GBMSAPI_Example_Util.GetDiagStringsFromDiagMask(
                                            GBMSAPI_Example_Globals.LastDiagnosticValue);
                                        if ((DiagList != null) && (DiagList.Count > 0))
                                        {
                                            for (int i = 0; i < DiagList.Count; i++)
                                            {
                                                this.DiagnosticMessageListBox.Items.Add(DiagList[i]);
                                            }
                                        }
                                    }

                                    this.ImSXTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeX;
                                    this.ImSYTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeY;
                                    this.CurrentFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameCurrentFrameRate.ToString("F2");
                                    this.NominalFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameNominalFrameRate.ToString("F2");
                                    this.AutoCapturePhaseTextBox.Text =
                                        GBMSAPI_Example_Util.GetAutoCapturePhaseStringFromValue(GBMSAPI_Example_Globals.AutoCapturePhase);

                                    if (GBMSAPI_Example_Globals.AutoCapturePhase == GBMSAPI_NET_AutoCapturePhase.GBMSAPI_NET_ACP_BEST_IMAGE_STOP)
                                    {
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                        this.StartStopButton.Enabled = false;
                                    }
                                    this.ContrastTextBox.Text = "" + GBMSAPI_Example_Globals.ImageContrast;
                                    this.SizeTextBox.Text = "" + GBMSAPI_Example_Globals.ImageSize;
                                    this.HLPCompletenessTextBox.Text = "" + GBMSAPI_Example_Globals.HalfLowerPalmCompleteness;

                                    // ver 3.1.0.1
                                    this.tbDryAreaPercent.Text = "" + GBMSAPI_Example_Globals.DryFingerPercent;
                                    this.tbWetAreaPercent.Text = "" + GBMSAPI_Example_Globals.WetFingerPercent;
                                    // ver 3.1.0.1

                                    // check acquisition end
                                    if (GBMSAPI_Example_Globals.AcquisitionEnded == true)
                                    {
                                        // ver 4.0.0.0: fake fingerprint
                                        uint devFeatures = 0;
                                        GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out devFeatures);
                                        if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HW_ANTIFAKE) != 0)
                                        {
                                            if (GBMSAPI_Example_Globals.HwFfdError != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                if (GBMSAPI_Example_Globals.HwFfdError == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_UNAVAILABLE_OPTION)
                                                {
                                                    String isFakeFinger = "HW FFD RESULT NOT AVAILABLE (acquisition manually stopped)";
                                                    this.DiagnosticMessageListBox.Items.Add(isFakeFinger);
                                                }
                                                else
                                                {
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "GBMSAPI_HardwareFakeFingerDetection");
                                                }
                                            }
                                            else
                                            {
                                                if (GBMSAPI_Example_Globals.HwFfdFlag)
                                                {
                                                    List<String> DiagList = GBMSAPI_Example_Util.GetFfdDiagStringsFromDiagMask(
                                                        GBMSAPI_Example_Globals.HwFfdDiagnosticValue, false);
                                                    if ((DiagList != null) && (DiagList.Count > 0))
                                                    {
                                                        for (int i = 0; i < DiagList.Count; i++)
                                                        {
                                                            this.DiagnosticMessageListBox.Items.Add(DiagList[i]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_SW_ANTIFAKE) != 0)
                                        {
                                            String isFakeFinger = GBMSAPI_Example_Globals.SwFfdFlag ? "FFD: Finger Is Fake" : "FFD: Finger Is Not Fake";
                                            this.DiagnosticMessageListBox.Items.Add(isFakeFinger);
                                        }
                                        // end ver 4.0.0.0: fake fingerprint
                                        // return to WAIT_START
                                        NextState = WAIT_START_STATUS;
                                        // check stop type
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_STOP_TYPE) == 0)
                                        {
                                            // manual
                                            this.StopTypeTextBox.Text = "MANUAL STOP";
                                            UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                             GBMSAPI_Example_Globals.ObjToScan);
                                            if (GBMSAPI_Example_Util.IsRolled(ObjType))
                                            {
                                                if ((GBMSAPI_Example_Globals.OptionMask &
                                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) == 0)
                                                    ShowRotationNotCompletedMessage();
                                            }
                                            else if
                                                (
                                                    ((GBMSAPI_Example_Globals.OptionMask &
                                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                                                )
                                            {
                                                MessageBox.Show("Image capture manually stopped before automatic stop: check image quality",
                                                    "WARNING");
                                            }
                                        }
                                        else
                                        {
                                            this.StopTypeTextBox.Text = "AUTO STOP";
                                            if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_TIMEOUT) != 0)
                                            {
                                                MessageBox.Show("ACQUISITION TIMEOUT HAS EXPIRED", "WARNING");
                                            }
                                        }

                                        // If LCD supported, enable OK button on stop screen									
                                        if ((ExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                        {
                                            GBMSAPI_Example_Globals.DisplayImageEvaluationParameters();
                                        }

                                        GBMSAPI_Example_Globals.AcquisitionEnded = false;
                                        // this.DiagnosticMessageListBox.Items.Add("AcquisitionEnded");
                                    }
                                    this.Refresh();

                                    GBMSAPI_Example_Globals.BusyFrame = false;

                                    // switch to next state
                                    GBMSAPI_Example_Globals.AcquisitionStatus = NextState;
                                }
                            }

                            this.Update();
                            break;
                        }

                    case WAIT_ACQUISITION_END_STATUS:
                        {
                            if (GBMSAPI_Example_Globals.PreviousAcquisitionStatus != WAIT_ACQUISITION_END_STATUS)
                            {
                                this.DiagnosticMessageListBox.Items.Clear();
                                // this.DiagnosticMessageListBox.Items.Add("WAIT_ACQUISITION_END_STATUS");
                            }
                            GBMSAPI_Example_Globals.PreviousAcquisitionStatus = WAIT_ACQUISITION_END_STATUS;
                            this.WaitFirstFrameLabel.Visible = false;
                            this.PreviewLabel.Visible = false;
                            this.StartStopButton.Text = "Stop";

                            uint ExternalEquipment;
                            int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetOptionalExternalEquipment(
                                out ExternalEquipment);

                            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                            {
                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                    RetVal, "WAIT_ACQUISITION_END_STATUS");
                                ExternalEquipment = 0;
                            }

                            // check errors
                            if (GBMSAPI_Example_Globals.LastErrorCode != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                            {
                                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                    GBMSAPI_Example_Globals.LastErrorCode, "WAIT_ACQUISITION_END_STATUS");
                                // return to WAIT_START
                                GBMSAPI_Example_Globals.LastErrorCode = GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR;
                                GBMSAPI_Example_Globals.AcquisitionStatus = WAIT_START_STATUS;
                                if ((ExternalEquipment & GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                {
                                    if (
                                        (GBMSAPI_Example_Globals.DisplayOptionMask & GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN) != 0
                                        // stop screen enabled after acquisition
                                    )
                                    {
                                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableOKButtonOnStopScreen();
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_ACQUISITION_END_STATUS");
                                        }
                                        // show message
                                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetCustomerMessageOnStopScreen(
                                            "ERROR IN ACQUISITION",
                                            GBMSAPI_NET_CustomMessagesDefines.GBMSAPI_NET_VILCD_CUSTOM_MSG_TYPE_ERROR_MSG);
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_ACQUISITION_END_STATUS");
                                        }
                                    }
                                    else
                                    {
                                        RetVal = GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableStartButtonOnLogoScreen();
                                        if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                        {
                                            GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                RetVal, "WAIT_ACQUISITION_END_STATUS");
                                        }
                                    }
                                }
                            }
                            else if (GBMSAPI_Example_Globals.AcquisitionButtonPressed == true)
                            {
                                UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                    GBMSAPI_Example_Globals.ObjToScan);

                                if ((GBMSAPI_Example_Globals.OptionMask &
                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) != 0 &&
                                    // VER 3.1.0.0: check all rolled objects
                                    (GBMSAPI_Example_Util.IsRolled(ObjType))
                                    // end VER 3.1.0.0: check all rolled objects
                                )
                                {
                                    int RetStopPreview = GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_ROLL_StopPreview();
                                    if (RetStopPreview != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                    {
                                        GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                    }
                                }
                                else
                                {
                                    GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                }
                                this.StartStopButton.Enabled = false;
                                GBMSAPI_Example_Globals.AcquisitionButtonPressed = false;
                            }
                            else
                            {
                                if (GBMSAPI_Example_Globals.BusyFrame == true)
                                {
                                    UInt32 NextState;
                                    // this.DiagnosticMessageListBox.Items.Add("BusyFrame");
                                    // a valid frame has been acquired
                                    // visualize frame
                                    // check acquisition phase
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        // EAVY ERROR: CANNOT SWITCH FROM ACQUISITION IN PROGRESS BACK TO
                                        // PREVIEW PHASE
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForPreview;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForPreview;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.PreviewImage;
                                        this.PreviewLabel.Visible = true;
                                        GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                    }
                                    else
                                    {
                                        // ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Location = GBMSAPI_Example_Globals.AcqWinPaintBoxLocationForAcq;
                                        this.AcquiredImagePictureBox.Size = GBMSAPI_Example_Globals.AcqWinPaintBoxSizeForAcq;
                                        // end ver 3.1.0.0 size and location for paint box
                                        this.AcquiredImagePictureBox.Image = GBMSAPI_Example_Globals.FullResImage;
                                        this.PreviewLabel.Visible = false;
                                        NextState = WAIT_ACQUISITION_END_STATUS;
                                    }
                                    // check image resolution
                                    if ((GBMSAPI_Example_Globals.LastEventInfo &
                                        GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
                                    {
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_PREVIEW_RES) == 0)
                                        {
                                            this.ImageResTextBox.Text = "LOW PREVIEW RES";
                                        }
                                        else
                                        {
                                            this.ImageResTextBox.Text = "FULL PREVIEW RES";
                                        }
                                    }
                                    else
                                    {
                                        this.ImageResTextBox.Text = "NA";
                                    }

                                    // show diagnostic
                                    this.DiagnosticMessageListBox.Items.Clear();
                                    if (GBMSAPI_Example_Globals.LastDiagnosticValue != 0)
                                    {
                                        List<String> DiagList = GBMSAPI_Example_Util.GetDiagStringsFromDiagMask(
                                            GBMSAPI_Example_Globals.LastDiagnosticValue);
                                        if ((DiagList != null) && (DiagList.Count > 0))
                                        {
                                            for (int i = 0; i < DiagList.Count; i++)
                                            {
                                                this.DiagnosticMessageListBox.Items.Add(DiagList[i]);
                                            }
                                        }
                                    }

                                    this.ImSXTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeX;
                                    this.ImSYTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameSizeY;
                                    this.CurrentFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameCurrentFrameRate.ToString("F2");
                                    this.NominalFrameRateTextBox.Text = "" + GBMSAPI_Example_Globals.LastFrameNominalFrameRate.ToString("F2");
                                    this.AutoCapturePhaseTextBox.Text =
                                        GBMSAPI_Example_Util.GetAutoCapturePhaseStringFromValue(GBMSAPI_Example_Globals.AutoCapturePhase);
                                    if (GBMSAPI_Example_Globals.AutoCapturePhase ==
                                        GBMSAPI_NET_AutoCapturePhase.GBMSAPI_NET_ACP_BEST_IMAGE_STOP)
                                    {
                                        NextState = WAIT_START_STATUS;
                                        this.StartStopButton.Enabled = false;
                                    }
                                    this.ContrastTextBox.Text = "" + GBMSAPI_Example_Globals.ImageContrast;
                                    this.SizeTextBox.Text = "" + GBMSAPI_Example_Globals.ImageSize;
                                    // ver 4.0.0.1
                                    if ((GBMSAPI_Example_Util.IsRolled(GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                        GBMSAPI_Example_Globals.ObjToScan))))
                                    {
                                        double RollToFlatRatio = (double)((double)GBMSAPI_Example_Globals.ImageSize) / ((double)GBMSAPI_Example_Globals.FlatFingerprintSize);
                                        this.tbRolledFlatRatio.Text = "" + RollToFlatRatio.ToString("F2");
                                    }
                                    else
                                    {
                                        this.tbRolledFlatRatio.Text = "";
                                    }
                                    // end ver 4.0.0.1
                                    this.HLPCompletenessTextBox.Text = "" + GBMSAPI_Example_Globals.HalfLowerPalmCompleteness;

                                    // ver 3.1.0.1
                                    this.tbDryAreaPercent.Text = "" + GBMSAPI_Example_Globals.DryFingerPercent;
                                    this.tbWetAreaPercent.Text = "" + GBMSAPI_Example_Globals.WetFingerPercent;
                                    // ver 3.1.0.1

                                    // check acquisition end
                                    if (GBMSAPI_Example_Globals.AcquisitionEnded == true)
                                    {
                                        // ver 4.0.0.0: fake fingerprint
                                        uint devFeatures = 0;
                                        GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out devFeatures);
                                        if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_HW_ANTIFAKE) != 0)
                                        {
                                            if (GBMSAPI_Example_Globals.HwFfdError != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                            {
                                                if (GBMSAPI_Example_Globals.HwFfdError == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_UNAVAILABLE_OPTION)
                                                {
                                                    String isFakeFinger = "HW FFD RESULT NOT AVAILABLE (acquisition manually stopped)";
                                                    this.DiagnosticMessageListBox.Items.Add(isFakeFinger);
                                                }
                                                else
                                                {
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "GBMSAPI_HardwareFakeFingerDetection");
                                                }
                                            }
                                            else
                                            {
                                                if (GBMSAPI_Example_Globals.HwFfdFlag)
                                                {
                                                    List<String> DiagList = GBMSAPI_Example_Util.GetFfdDiagStringsFromDiagMask(
                                                        GBMSAPI_Example_Globals.HwFfdDiagnosticValue, false);
                                                    if ((DiagList != null) && (DiagList.Count > 0))
                                                    {
                                                        for (int i = 0; i < DiagList.Count; i++)
                                                        {
                                                            this.DiagnosticMessageListBox.Items.Add(DiagList[i]);
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        if ((devFeatures & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_SW_ANTIFAKE) != 0)
                                        {
                                            String isFakeFinger = GBMSAPI_Example_Globals.SwFfdFlag ? "FFD: Finger Is Fake" : "FFD: Finger Is Not Fake";
                                            this.DiagnosticMessageListBox.Items.Add(isFakeFinger);
                                        }
                                        // end ver 4.0.0.0: fake fingerprint
                                        // return to WAIT_START
                                        NextState = WAIT_START_STATUS;
                                        // this.DiagnosticMessageListBox.Items.Add("AcquisitionEnded");
                                        // check stop type
                                        if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_STOP_TYPE) == 0)
                                        {
                                            // manual
                                            this.StopTypeTextBox.Text = "MANUAL STOP";
                                            UInt32 ObjType = GBMSAPI_NET_ScanObjectsUtilities.GBMSAPI_NET_GetTypeFromObject(
                                            GBMSAPI_Example_Globals.ObjToScan);
                                            if (GBMSAPI_Example_Util.IsRolled(ObjType))
                                            {
                                                if ((GBMSAPI_Example_Globals.OptionMask &
                                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_MANUAL_ROLL_PREVIEW_STOP) == 0)
                                                    ShowRotationNotCompletedMessage();
                                            }
                                            else if
                                                (
                                                    ((GBMSAPI_Example_Globals.OptionMask &
                                                    GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_AUTOCAPTURE) != 0)
                                                )
                                            {
                                                MessageBox.Show("Image capture manually stopped before automatic stop: check image quality",
                                                    "WARNING");
                                            }
                                        }
                                        else
                                        {
                                            this.StopTypeTextBox.Text = "AUTO STOP";
                                            if ((GBMSAPI_Example_Globals.LastEventInfo &
                                            GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_TIMEOUT) != 0)
                                            {
                                                MessageBox.Show("ACQUISITION TIMEOUT HAS EXPIRED", "WARNING");
                                            }
                                        }

                                        DrawRollInfo = true;
                                        this.ArtefactsSizeTextBox.Text = "" + GBMSAPI_Example_Globals.RolledArtefactSize;
                                        this.Update();

                                        // If LCD supported, enable OK button on stop screen
                                        if ((ExternalEquipment &
                                            GBMSAPI_NET_OptionalExternalEquipment.GBMSAPI_NET_OED_VUI_LCD) != 0)
                                        {
                                            if (
                                                (GBMSAPI_Example_Globals.DisplayOptionMask &
                                                GBMSAPI_NET_DisplayOptions.GBMSAPI_NET_DO_FINAL_SCREEN) != 0
                                                // stop screen enabled after acquisition
                                            )
                                            {
                                                // show message
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_SetPreloadedMessageOnStopScreen(
                                                    GBMSAPI_NET_PreloadedGeneralMessages.GBMSAPI_NET_VILCD_MSG_ACQUISITION_SUCCESSFUL,
                                                    GBMSAPI_NET_PreloadedMessagesArea.GBMSAPI_NET_VILCD_MSG_AREA);
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_ACQUISITION_END_STATUS, GBMSAPI_NET_VUI_LCD_SetPreloadedMessageOnStopScreen");
                                                }
                                                // set image evaluation parameters
                                                GBMSAPI_Example_Globals.DisplayImageEvaluationParameters();

                                                // enable OK button on stop screen
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableOKButtonOnStopScreen();
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                }
                                            }
                                            else
                                            {
                                                RetVal =
                                                    GBMSAPI_NET_ExternalDevicesControlRoutines.GBMSAPI_NET_VUI_LCD_EnableStartButtonOnLogoScreen();
                                                if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
                                                {
                                                    GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                                                        RetVal, "WAIT_ACQUISITION_END_STATUS");
                                                }
                                            }
                                        }

                                        GBMSAPI_Example_Globals.AcquisitionEnded = false;
                                    }

                                    this.Refresh();
                                    GBMSAPI_Example_Globals.BusyFrame = false;

                                    // switch to next state
                                    GBMSAPI_Example_Globals.AcquisitionStatus = NextState;
                                }
                            }

                            this.Update();
                            break;
                        }

                }
            }
            catch (Exception ex)
            {
                GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                MessageBox.Show("Exception in AcquisitionStateManagement: " + ex.Message);
                this.Close();
            }
        }

        private void AcquiredImagePictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (GBMSAPI_Example_Globals.BusyFrame)
            {
                try
                {
                    // Adjust picture box to frame size
                    float XFactor = ((float)(this.AcquiredImagePictureBox.Width) / GBMSAPI_Example_Globals.LastFrameSizeX);
                    float YFactor = ((float)(this.AcquiredImagePictureBox.Height) / GBMSAPI_Example_Globals.LastFrameSizeY);

                    GBMSAPI_Example_Globals.PaintNumber++;

                    // check for clipping
                    if (
                     GBMSAPI_Example_Globals.ClippingRegionPosX >= 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionPosY >= 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionSizeX != 0 &&
                     GBMSAPI_Example_Globals.ClippingRegionSizeY != 0
                     )
                    {
                        float ClipPosX = GBMSAPI_Example_Globals.ClippingRegionPosX * XFactor;
                        float ClipPosY = GBMSAPI_Example_Globals.ClippingRegionPosY * YFactor;
                        float ClipSizeX = GBMSAPI_Example_Globals.ClippingRegionSizeX * XFactor;
                        float ClipSizeY = GBMSAPI_Example_Globals.ClippingRegionSizeY * YFactor;

                        System.Drawing.Pen ClipPen = new Pen(Color.Blue, (float)3.0);
                        e.Graphics.DrawRectangle(
                            ClipPen,
                            ClipPosX,
                            ClipPosY,
                            ClipSizeX,
                            ClipSizeY
                        );
                    }

                    // at the end of rolled acquisition, draw roll info
                    if (this.DrawRollInfo)
                    {
                        if (GBMSAPI_Example_Globals.MarkerFrame != null)
                        {
                            System.IO.File.WriteAllBytes("MarkerFrame.raw", GBMSAPI_Example_Globals.MarkerFrame);
                            System.Drawing.Pen WipedPen = new Pen(Color.Red, (float)1.0);
                            System.Drawing.Pen ArtefactPen = new Pen(Color.Green, (float)1.0);
                            for (int x = 0; x < GBMSAPI_Example_Globals.LastFrameSizeX; x++)
                            {
                                for (int y = 0; y < GBMSAPI_Example_Globals.LastFrameSizeY; y++)
                                {
                                    int PositionInVector = x + y * GBMSAPI_Example_Globals.LastFrameSizeX;
                                    if ((GBMSAPI_Example_Globals.MarkerFrame[PositionInVector] &
                                        GBMSAPI_NET_ArtefactRegionMarkers.GBMSAPI_NET_ARTEFACT_REGION_MARKED_BORDER) != 0)
                                    {
                                        float xPoint = x * XFactor;
                                        float yPoint = y * YFactor;
                                        e.Graphics.DrawRectangle(ArtefactPen, xPoint, yPoint, (float)2.0, (float)2.0);
                                    }
                                    if ((GBMSAPI_Example_Globals.MarkerFrame[PositionInVector] &
                                        GBMSAPI_NET_ArtefactRegionMarkers.GBMSAPI_NET_ARTEFACT_REGION_WIPED_BORDER) != 0)
                                    {
                                        float xPoint = x * XFactor;
                                        float yPoint = y * YFactor;
                                        e.Graphics.DrawRectangle(WipedPen, xPoint, yPoint, (float)2.0, (float)2.0);
                                    }
                                }
                            }
                        }
                    }

                    // Ver 3.1.0.0
                    uint DeviceFeaturesMask;
                    int RetVal = GBMSAPI_NET_DeviceCharacteristicsRoutines.GBMSAPI_NET_GetDeviceFeatures(out DeviceFeaturesMask);
                    if (RetVal == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR &&
                        ((DeviceFeaturesMask & GBMSAPI_NET_DeviceFeatures.GBMSAPI_NET_DF_ROLL_OBJECT_STRIPE) != 0)
                        )
                    {
                        int leftX, rightX;
                        RetVal = GBMSAPI_NET_ValidFrameAcquiredRoutines.GBMSAPI_NET_ROLL_GetCurrentStripeCoord(out leftX, out rightX);
                        if (
                            (RetVal == GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR) &&
                            (leftX >= 0) && (rightX >= 0)
                            )
                        {
                            System.Drawing.Pen SlicePen = new Pen(Color.Aquamarine, (float)1.0);
                            e.Graphics.DrawRectangle(SlicePen, (int)(leftX * XFactor), 0, (float)((rightX - leftX) * XFactor),
                                (float)this.AcquiredImagePictureBox.Height);
                            String dbgStr = "L " + leftX + "; R " + rightX;
                            this.DiagnosticMessageListBox.Items.Add(dbgStr);
                        }
                    }
                    // end Ver 3.1.0.0
                }
                catch (Exception ex)
                {
                    GBMSAPI_NET.GBMSAPI_NET_LibraryFunctions.GBMSAPI_NET_ScanningRoutines.GBMSAPI_NET_StopAcquisition();
                    MessageBox.Show("Exception in AcquiredImagePictureBox_Paint: " + ex.Message);
                    this.Close();
                }
            }
        }

        private void AcquisitionManagementGeneralTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                this.AcquisitionManagementGeneralTimer.Stop();

                TimeSpan x = DateTime.Now.Subtract(this.DelayBeforeStartTimer);

                this.AcquiredFramesNumTextBox.Text = "" + GBMSAPI_Example_Globals.AcquiredFramesNumber;
                this.LostFramesNumTextBox.Text = "" + GBMSAPI_Example_Globals.LostFramesNumber;

                if (x.TotalMilliseconds > 1000)
                {
                    if (this.FirstStart == true)
                    {
                        this.StartStopButton_Click(null, null);
                        this.FirstStart = false;
                    }
                }

                this.AcquisitionStateManagement();

                this.AcquisitionManagementGeneralTimer.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Exception in management timer: " + ex.Message);
                this.Close();
            }
        }

        private void AcquisitionForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            ReleaseResources();
        }

        private void cbExcludeFinalization_CheckedChanged(object sender, EventArgs e)
        {
            GBMSAPI_Example_Globals.UseImageFinalization = !(this.cbExcludeFinalization.Checked);
        }

      
        private void bUpdateBackgroundImage_Click(object sender, EventArgs e)
        {
            UInt32 Diag;
            int RetVal;
            byte[] bgImage = new byte[GBMSAPI_Example_Globals.LastFrameSizeX *
                    GBMSAPI_Example_Globals.LastFrameSizeY]; ;
            if ((GBMSAPI_Example_Globals.LastEventInfo & GBMSAPI_NET_EventInfo.GBMSAPI_NET_EI_ACQUISITION_PHASE) == 0)
            {
                Marshal.Copy(GBMSAPI_Example_Globals.AcquisitionPreviewBuffer, bgImage, 0, bgImage.Length);
            }
            else
            {
                Marshal.Copy(GBMSAPI_Example_Globals.AcquisitionFullResBuffer, bgImage, 0, bgImage.Length);
            }
            System.IO.File.WriteAllBytes("backImage.raw", bgImage);
            RetVal = GBMSAPI_NET_ValidFrameAcquiredRoutines.GBMSAPI_NET_UpdateBackgroundImage(
                bgImage, out Diag);
            if (RetVal != GBMSAPI_NET_ErrorCodes.GBMSAPI_NET_ERROR_CODE_NO_ERROR)
            {
                GBMSAPI_Example_Util.GBMSAPI_Example_ManageErrors(
                    RetVal, "bUpdateBackgroundImage_Click, GBMSAPI_NET_UpdateBackgroundImage");
            }
        }
      
        public static void ShowRotationNotCompletedMessage()
        {
            if (!((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_EXTERNAL_ROLL_COMPOSITION) != 0)
                //|| ((GBMSAPI_Example_Globals.OptionMask & GBMSAPI_NET_AcquisitionOptions.GBMSAPI_NET_AO_ADAPT_ROLL_AREA_POSITION) != 0)
            )
            {
                MessageBox.Show("Rotation not completed", "WARNING");
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
