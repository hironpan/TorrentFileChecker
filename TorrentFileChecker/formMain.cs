//Author: hironpan
//QQ: 12497920
//引用：https://github.com/Krusen/BencodeNET

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using BencodeNET.Objects;
using BencodeNET.Parsing;
using BencodeNET.Torrents;
using System.Windows.Forms;

namespace TorrentFileChecker
{
    public partial class formMain : Form
    {
        public formMain()
        {
            InitializeComponent();
            typeof(Panel).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).SetValue(pbMain, true, null);
            tbxTorrent.MouseDown += TextBox_MouseDown;
            tbxFile.MouseDown += TextBox_MouseDown;
            tbxMD5.MouseDown += TextBox_MouseDown;
            tbxSHA1.MouseDown += TextBox_MouseDown;
            tbxSHA256.MouseDown += TextBox_MouseDown;
            tbxTorrent.KeyDown += TextBox_KeyDown;
            tbxFile.KeyDown += TextBox_KeyDown;
            tbxMD5.KeyDown += TextBox_KeyDown;
            tbxSHA1.KeyDown += TextBox_KeyDown;
            tbxSHA256.KeyDown += TextBox_KeyDown;
            this.SizeChanged += formMain_SizeChanged;
        }

        private void formMain_SizeChanged(object sender, EventArgs e)
        {
            UpdateProgressBar();
        }

        private bool ArrayEquals(byte[] array1, byte[] array2)
        {
            if (array1 != null && array2 != null)
                return ArrayEquals(array1, 0, array1.Length, array2, 0, array2.Length);
            else
                return false;
        }
        private bool ArrayEquals(byte[] array1, int offset1, int length1, byte[] array2, int offset2, int length2)
        {
            bool retVal = true;
            if (array1 != null && array2 != null && length1 == length2)
            {
                while (length1 > 0)
                {
                    retVal = array1[offset1] == array2[offset2];
                    if (!retVal)
                        break;
                    offset1++;
                    offset2++;
                    length1--;
                }
            }
            else
                retVal = false;
            return retVal;
        }

        private const int _FileBufferSize = 4096 * 4;
        private bool _ProcCancel = true;

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !_ProcCancel;
        }

        private void btnTorrent_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (tbxTorrent.TextLength > 0)
                try { dlg.InitialDirectory = System.IO.Directory.GetParent(tbxTorrent.Text).FullName; } catch { }
            dlg.Filter = "种子文件|*.torrent";
            dlg.Title = "打开";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbxTorrent.Text = dlg.FileName;
                tbxMD5.Clear();
                tbxSHA1.Clear();
                tbxSHA256.Clear();
                pbMain.BackgroundImage = null;
            }
        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog();
            if (tbxFile.TextLength > 0)
                try { dlg.InitialDirectory = System.IO.Directory.GetParent(tbxFile.Text).FullName; } catch { }
            dlg.Filter = "所有文件|*.*";
            dlg.Title = "打开";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                tbxFile.Text = dlg.FileName;
                tbxMD5.Clear();
                tbxSHA1.Clear();
                tbxSHA256.Clear();
                pbMain.BackgroundImage = null;
            }
        }

        private void btnCheck_Click(object sender, EventArgs e)
        {
            if (btnCheck.Text == "校验")
            {
                if (!System.IO.File.Exists(tbxTorrent.Text))
                    MessageBox.Show("请选择种子文件！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else if (!System.IO.File.Exists(tbxFile.Text))
                    MessageBox.Show("请选择待校验的文件！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                else
                {
                    btnTorrent.Enabled = false;
                    btnFile.Enabled = false;
                    tbxMD5.Clear();
                    tbxSHA1.Clear();
                    tbxSHA256.Clear();
                    pbMain.BackgroundImage = null;
                    btnCheck.Text = "停止";
                    _ProcCancel = false;
                    System.Threading.ThreadPool.QueueUserWorkItem(ProcCheck);
                }
            }
            else
            {
                _ProcCancel = true;
                btnCheck.Enabled = false;
            }
        }

        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {
                var tbx = sender as TextBox;
                if (tbx.TextLength > 0)
                    Clipboard.SetText(tbx.Text);
            }
        }
        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.A)
                (sender as TextBox).SelectAll();
        }


        byte[] blockResult = null; int blockIndexBg = 0, blockIndexEd = 0;

        private byte[] pbData = null, pbDataOld = null; int[] pbRIdxRg = null;
        private Bitmap pbImg = null;
        private SolidBrush brush0 = new SolidBrush(Color.DarkGray), brush1 = new SolidBrush(Color.Blue), brush2 = new SolidBrush(Color.Red);
        private void UpdateProgressBar()
        {
            if (pbData != null && pbData.Length != pbMain.Width)
            {
                pbData = null;
                pbMain.BackgroundImage = null;
                pbImg.Dispose();
                pbImg = null;
            }
            if (pbData == null)
            {
                pbData = new byte[pbMain.Width];
                pbDataOld = new byte[pbData.Length]; pbDataOld[0] = 255;
                pbRIdxRg = new int[pbData.Length + 1];
                pbImg = new Bitmap(pbData.Length, pbMain.Height);
            }
            int pbIdx = 0;
            if (blockResult != null)
            {
                int brIdx;
                var blockResultLength = blockIndexEd - blockIndexBg + 1;
                pbIdx = 0;
                while (pbIdx < pbData.Length)
                {
                    pbRIdxRg[pbIdx] = blockIndexBg + pbIdx * blockResultLength / pbData.Length;
                    pbIdx++;
                }
                pbRIdxRg[pbIdx] = blockIndexEd;
                pbIdx = 0;
                while (pbIdx < pbData.Length)
                {
                    brIdx = pbRIdxRg[pbIdx];
                    pbData[pbIdx] = blockResult[brIdx];
                    while (brIdx <= pbRIdxRg[pbIdx + 1])
                    {
                        if (pbData[pbIdx] == 0)
                        {
                            if (blockResult[brIdx] == 2)
                                pbData[pbIdx] = 2;
                        }
                        else if (pbData[pbIdx] == 1)
                        {
                            if (blockResult[brIdx] == 2)
                                pbData[pbIdx] = 2;
                            else if (blockResult[brIdx] == 0)
                                pbData[pbIdx] = 0;
                        }
                        else
                            break;
                        brIdx++;
                    }
                    pbIdx++;
                }
            }
            if (ArrayEquals(pbData, pbDataOld))
                return;
            Array.Copy(pbData, 0, pbDataOld, 0, pbData.Length);
            using (var g = Graphics.FromImage(pbImg))
            {
                g.FillRectangle(brush0, new Rectangle(0, 0, pbImg.Width, pbImg.Height));
                pbIdx = 0;
                while (pbIdx < pbData.Length)
                {
                    if (pbData[pbIdx] == 0)
                        g.FillRectangle(brush0, new Rectangle(pbIdx, 0, 1, pbImg.Height));
                    else if (pbData[pbIdx] == 1)
                        g.FillRectangle(brush1, new Rectangle(pbIdx, 0, 1, pbImg.Height));
                    else
                        g.FillRectangle(brush2, new Rectangle(pbIdx, 0, 1, pbImg.Height));
                    pbIdx++;
                }
            }
            pbMain.BackgroundImage = pbImg;
            pbMain.Refresh();
        }





        private void ProcCheck(object state)
        {
            string pathTorr = (string)this.Invoke(new Func<string>(() => tbxTorrent.Text)), pathFile = (string)this.Invoke(new Func<string>(() => tbxFile.Text));
            Torrent infoTorr; System.IO.FileInfo infoFile;
            int blockIdx = 0;
            System.Security.Cryptography.HashAlgorithm fileMD5 = new System.Security.Cryptography.MD5CryptoServiceProvider(), fileSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider(), fileSHA256 = new System.Security.Cryptography.SHA256CryptoServiceProvider(); byte[] fileMD5_OBuff = new byte[16 * 1024], fileSHA1_OBuff = new byte[16 * 1024], fileSHA256_OBuff = new byte[16 * 1024];
            System.Security.Cryptography.HashAlgorithm blockSHA1 = null; byte[] blockSHA1_OBuff = null; int blockSHA1_TrLength = 0;
            //文件流读取的数据缓存 缓存数据起始索引 缓存数据长度
            byte[] fileData; int fileDataIdx, fileDataLen, fileDataLenUse;
            try
            {
                infoTorr = new BencodeParser().Parse<Torrent>(pathTorr);
                if (infoTorr.Pieces == null || infoTorr.Pieces.Length == 0 || infoTorr.Pieces.Length % 20 != 0)
                    throw new Exception("种子文件区块Hash信息有误！");
                infoFile = new System.IO.FileInfo(pathFile);
                if (infoFile.Length == 0)
                    throw new Exception("待校验的文件大小为0！");
            }
            catch (Exception ex)
            {
                this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), ex.Message);
                infoTorr = null;
                infoFile = null;
            }
            if (!_ProcCancel)
            {
                blockResult = new byte[infoTorr.Pieces.Length / 20];
                //分析待校验的文件的首尾区块
                if (infoTorr.FileMode == TorrentFileMode.Single && infoTorr.File != null)
                {
                    blockIndexBg = 0; blockIndexEd = blockResult.Length - 1;
                    if (infoTorr.File.FileSize != infoFile.Length)
                        this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), "所选文件大小与种子信息中的文件大小不匹配！");
                    else
                    {
                        fileData = new byte[_FileBufferSize]; fileDataIdx = 0; fileDataLen = 0;
                        blockIdx = 0;
                        try
                        {
                            using (var fileStream = new System.IO.FileStream(pathFile, System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                            {
                                while (!_ProcCancel)
                                {
                                    if (fileDataLen == 0)
                                    {
                                        fileDataIdx = 0;
                                        fileDataLen = fileStream.Read(fileData, fileDataIdx, fileData.Length - fileDataIdx);
                                        fileMD5.TransformBlock(fileData, fileDataIdx, fileDataLen, fileMD5_OBuff, 0);
                                        fileSHA1.TransformBlock(fileData, fileDataIdx, fileDataLen, fileSHA1_OBuff, 0);
                                        fileSHA256.TransformBlock(fileData, fileDataIdx, fileDataLen, fileSHA256_OBuff, 0);
                                    }
                                    if (fileDataLen == 0)
                                        break;
                                    if (blockSHA1 == null)
                                    {
                                        blockSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                                        blockSHA1_OBuff = new byte[16 * 1024];
                                        blockSHA1_TrLength = 0;
                                    }
                                    fileDataLenUse = Math.Min(fileDataLen, (int)(infoTorr.PieceSize - blockSHA1_TrLength));
                                    blockSHA1.TransformBlock(fileData, fileDataIdx, fileDataLenUse, blockSHA1_OBuff, 0);
                                    blockSHA1_TrLength += fileDataLenUse;
                                    fileDataIdx += fileDataLenUse;
                                    fileDataLen -= fileDataLenUse;
                                    if (blockSHA1_TrLength == infoTorr.PieceSize)
                                    {
                                        blockSHA1.TransformFinalBlock(new byte[] { }, 0, 0);
                                        blockResult[blockIdx] = ArrayEquals(blockSHA1.Hash, 0, 20, infoTorr.Pieces, 20 * blockIdx, 20) ? (byte)1 : (byte)2;
                                        blockSHA1.Clear();
                                        blockSHA1 = null;
                                        blockSHA1_OBuff = null;
                                        blockSHA1_TrLength = 0;
                                        blockIdx++;
                                        this.Invoke(new Action(UpdateProgressBar));
                                    }
                                }
                            }
                            if (_ProcCancel == false && blockSHA1 != null)
                            {
                                blockSHA1.TransformFinalBlock(new byte[] { }, 0, 0);
                                blockResult[blockIdx] = ArrayEquals(blockSHA1.Hash, 0, 20, infoTorr.Pieces, 20 * blockIdx, 20) ? (byte)1 : (byte)2;
                                blockSHA1.Clear();
                                blockSHA1 = null;
                                blockSHA1_OBuff = null;
                                blockSHA1_TrLength = 0;
                                blockIdx++;
                                this.Invoke(new Action(UpdateProgressBar));
                            }
                        }
                        catch (Exception ex)
                        {
                            this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), ex.Message);
                        }
                    }
                }
                else if (infoTorr.FileMode == TorrentFileMode.Multi && infoTorr.Files != null && infoTorr.Files.Count != 0)
                {
                    int tFileIndexBg = -1, tFileIndex = -1, tFileIndexEd = -1; long tPreFilesSize = 0;
                    int tFileIdx; var tFileIndexMatches = new List<int>();
                    //查找种子中对应的文件索引
                    for (tFileIdx = 0; tFileIdx < infoTorr.Files.Count; tFileIdx++)
                        if (infoTorr.Files[tFileIdx].FileSize == infoFile.Length)
                            tFileIndexMatches.Add(tFileIdx);
                    if (tFileIndexMatches.Count == 1)
                        tFileIndex = tFileIndexMatches[0];
                    else if (tFileIndexMatches.Count != 0)
                    {
                        var tFileIndexMatches2 = new List<int>();
                        for (tFileIdx = 0; tFileIdx < tFileIndexMatches.Count; tFileIdx++)
                            if (infoTorr.Files[tFileIndexMatches[tFileIdx]].FileName == infoFile.Name)
                                tFileIndexMatches2.Add(tFileIndexMatches[tFileIdx]);
                        if (tFileIndexMatches2.Count == 0)
                            for (tFileIdx = 0; tFileIdx < tFileIndexMatches.Count; tFileIdx++)
                                if (string.Equals(infoTorr.Files[tFileIndexMatches[tFileIdx]].FileName, infoFile.Name, StringComparison.OrdinalIgnoreCase))
                                    tFileIndexMatches2.Add(tFileIndexMatches[tFileIdx]);
                        if (tFileIndexMatches2.Count == 0)
                            for (tFileIdx = 0; tFileIdx < tFileIndexMatches.Count; tFileIdx++)
                                if (string.Equals(System.Text.RegularExpressions.Regex.Match(infoTorr.Files[tFileIndexMatches[tFileIdx]].FileName, @"\.[^\.]+\z").Value, infoFile.Extension, StringComparison.OrdinalIgnoreCase))
                                    tFileIndexMatches2.Add(tFileIndexMatches[tFileIdx]);
                        if (tFileIndexMatches2.Count == 1)
                            tFileIndex = tFileIndexMatches2[0];
                    }
                    if (tFileIndex == -1)
                        this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), "种子信息中未找到与所选文件匹配的项！");
                    else
                    {
                        //计算待校验文件所处数据流的位置
                        tFileIdx = 0;
                        while (tFileIdx != tFileIndex)
                        {
                            tPreFilesSize += infoTorr.Files[tFileIdx].FileSize;
                            tFileIdx++;
                        }
                        //待校验的文件，首区块需要的其他相关文件索引起始值
                        long tTmpSize = tPreFilesSize % infoTorr.PieceSize;
                        tFileIdx = tFileIndex;
                        while (tTmpSize > 0)
                        {
                            tFileIdx--;
                            tTmpSize -= infoTorr.Files[tFileIdx].FileSize;
                        }
                        tFileIndexBg = tFileIdx;
                        //待校验的文件，尾区块需要的其他相关文件索引终止值
                        tTmpSize = (tPreFilesSize + infoTorr.Files[tFileIndex].FileSize) % infoTorr.PieceSize;
                        if (tTmpSize > 0)
                            tTmpSize = infoTorr.PieceSize - tTmpSize;
                        tFileIdx = tFileIndex;
                        while (tTmpSize > 0)
                        {
                            if (tFileIdx + 1 < infoTorr.Files.Count)
                            {
                                tFileIdx++;
                                tTmpSize -= infoTorr.Files[tFileIdx].FileSize;
                            }
                            else
                                break;
                        }
                        tFileIndexEd = tFileIdx;
                        //初始化区块索引
                        blockIndexBg = (int)(tPreFilesSize / infoTorr.PieceSize);
                        blockIndexEd = (int)((tPreFilesSize + infoTorr.Files[tFileIndex].FileSize - 1) / infoTorr.PieceSize);
                        //自动检索首尾区块校验所需的文件，如果自动检索的文件不存在或信息有误，则让用户手动选择
                        string tmpStr; bool tmpBl; string[] pathArray = new string[infoTorr.Files.Count];
                        for (tFileIdx = 0; tFileIdx < pathArray.Length; tFileIdx++)
                            pathArray[tFileIdx] = null;
                        pathArray[tFileIndex] = pathFile;
                        if ((tFileIndexBg != tFileIndex || tFileIndexEd != tFileIndex) && pathFile.EndsWith(infoTorr.Files[tFileIndex].FullPath, StringComparison.OrdinalIgnoreCase))
                        {
                            tmpStr = pathFile.Substring(0, pathFile.Length - infoTorr.Files[tFileIndex].FullPath.Length);
                            for (tFileIdx = 0; tFileIdx < pathArray.Length; tFileIdx++)
                                if (tFileIdx != tFileIndex)
                                    pathArray[tFileIdx] = tmpStr + infoTorr.Files[tFileIdx].FullPath;
                            tFileIdx = tFileIndexBg;
                            while (tFileIdx < tFileIndexEd + 1)
                            {
                                if (tFileIdx != tFileIndex)
                                {
                                    tmpBl = false;
                                    if (System.IO.File.Exists(pathArray[tFileIdx]))
                                        try { tmpBl = new System.IO.FileInfo(pathArray[tFileIdx]).Length == infoTorr.Files[tFileIdx].FileSize; } catch { tmpBl = false; }
                                    if (!tmpBl)
                                    {
                                        for (tFileIdx = 0; tFileIdx < pathArray.Length; tFileIdx++)
                                            if (tFileIdx != tFileIndex)
                                                pathArray[tFileIdx] = null;
                                        break;
                                    }
                                }
                                tFileIdx++;
                            }
                        }
                        if (tFileIndexBg != tFileIndex && string.IsNullOrEmpty(pathArray[tFileIndexBg]))
                        {
                            tmpStr = $"待校验的文件起始 {infoTorr.PieceSize - tPreFilesSize % infoTorr.PieceSize} 字节数据需要以下文件配合校验：\r\n";
                            tFileIdx = tFileIndexBg;
                            while (tFileIdx < tFileIndex)
                            {
                                tmpStr += $"文件名：{infoTorr.Files[tFileIdx].FileName}，大小：{infoTorr.Files[tFileIdx].FileSize} 字节\r\n";
                                tFileIdx++;
                            }
                            this.Invoke(new Action<string>(msgTxt => MessageBox.Show(this, msgTxt, "", MessageBoxButtons.OK, MessageBoxIcon.Information)), tmpStr);
                            tFileIdx = tFileIndexBg;
                            while (tFileIdx < tFileIndex)
                            {
                                pathArray[tFileIdx] = (string)this.Invoke(new Func<string, long, string>(_SelectFile), infoTorr.Files[tFileIdx].FileName, infoTorr.Files[tFileIdx].FileSize);
                                if (string.IsNullOrEmpty(pathArray[tFileIdx]))
                                {
                                    pathArray[tFileIndexBg] = null;
                                    break;
                                }
                                tFileIdx++;
                            }
                        }
                        if (tFileIndexEd != tFileIndex && string.IsNullOrEmpty(pathArray[tFileIndexEd]))
                        {
                            tmpStr = $"待校验的文件末尾 {(tPreFilesSize + infoTorr.Files[tFileIndex].FileSize) % infoTorr.PieceSize} 字节数据需要以下文件配合校验：\r\n";
                            tFileIdx = tFileIndex;
                            while (tFileIdx < tFileIndexEd)
                            {
                                tFileIdx++;
                                tmpStr += $"文件名：{infoTorr.Files[tFileIdx].FileName}，大小：{infoTorr.Files[tFileIdx].FileSize} 字节\r\n";
                            }
                            this.Invoke(new Action<string>(msgTxt => MessageBox.Show(this, msgTxt, "", MessageBoxButtons.OK, MessageBoxIcon.Information)), tmpStr);
                            tFileIdx = tFileIndex;
                            while (tFileIdx < tFileIndexEd)
                            {
                                tFileIdx++;
                                pathArray[tFileIdx] = (string)this.Invoke(new Func<string, long, string>(_SelectFile), infoTorr.Files[tFileIdx].FileName, infoTorr.Files[tFileIdx].FileSize);
                                if (string.IsNullOrEmpty(pathArray[tFileIdx]))
                                {
                                    pathArray[tFileIndexEd] = null;
                                    break;
                                }
                            }
                        }
                        //校验文件
                        if (infoTorr.Files[tFileIndex].FileSize < infoTorr.PieceSize && (string.IsNullOrEmpty(pathArray[tFileIndexBg]) || string.IsNullOrEmpty(pathArray[tFileIndexEd])))
                            this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), "缺少必要的文件，无法执行文件校验！");
                        long seekOffset;
                        if (string.IsNullOrEmpty(pathArray[tFileIndexBg]))
                        {
                            seekOffset = infoTorr.PieceSize - (tPreFilesSize % infoTorr.PieceSize);
                            tFileIdx = tFileIndex;
                            blockIdx = blockIndexBg + 1;
                        }
                        else if (tFileIndexBg != tFileIndex)
                        {
                            tTmpSize = 0;
                            tFileIdx = 0;
                            while (tFileIdx != tFileIndexBg)
                            {
                                tTmpSize += infoTorr.Files[tFileIdx].FileSize;
                                tFileIdx++;
                            }
                            seekOffset = tTmpSize + infoTorr.Files[tFileIndexBg].FileSize - (tTmpSize + infoTorr.Files[tFileIndexBg].FileSize) % infoTorr.PieceSize - tTmpSize;
                            tFileIdx = tFileIndexBg;
                            blockIdx = blockIndexBg;
                        }
                        else
                        {
                            seekOffset = 0;
                            tFileIdx = tFileIndexBg;
                            blockIdx = blockIndexBg;
                        }
                        while (_ProcCancel == false && tFileIdx <= tFileIndexEd)
                        {
                            if (tFileIdx > tFileIndex && string.IsNullOrEmpty(pathArray[tFileIndexEd]))
                            {
                                blockResult[blockIdx] = 0;
                                blockSHA1?.Clear();
                                blockSHA1 = null;
                                blockSHA1_OBuff = null;
                                blockSHA1_TrLength = 0;
                                blockIdx++;
                                this.Invoke(new Action(UpdateProgressBar));
                                break;
                            }
                            fileData = new byte[_FileBufferSize]; fileDataIdx = 0; fileDataLen = 0;
                            try
                            {
                                using (var fileStream = new System.IO.FileStream(pathArray[tFileIdx], System.IO.FileMode.Open, System.IO.FileAccess.Read, System.IO.FileShare.Read))
                                {
                                    if (fileStream.Length != infoTorr.Files[tFileIdx].FileSize)
                                        throw new Exception($"文件名：{infoTorr.Files[tFileIdx].FileName}，大小：{infoTorr.Files[tFileIdx].FileSize} 字节\r\n选择的文件\"{pathArray[tFileIdx]}\"大小不匹配！");
                                    if (seekOffset != 0)
                                    {
                                        fileStream.Seek(seekOffset, System.IO.SeekOrigin.Begin);
                                        seekOffset = 0;
                                    }
                                    while (!_ProcCancel)
                                    {
                                        if (fileDataLen == 0)
                                        {
                                            fileDataIdx = 0;
                                            fileDataLen = fileStream.Read(fileData, fileDataIdx, fileData.Length - fileDataIdx);
                                            if (tFileIdx == tFileIndex)
                                            {
                                                fileMD5.TransformBlock(fileData, fileDataIdx, fileDataLen, fileMD5_OBuff, 0);
                                                fileSHA1.TransformBlock(fileData, fileDataIdx, fileDataLen, fileSHA1_OBuff, 0);
                                                fileSHA256.TransformBlock(fileData, fileDataIdx, fileDataLen, fileSHA256_OBuff, 0);
                                            }
                                        }
                                        if (fileDataLen == 0)
                                            break;
                                        if (blockSHA1 == null)
                                        {
                                            blockSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                                            blockSHA1_OBuff = new byte[16 * 1024];
                                            blockSHA1_TrLength = 0;
                                        }
                                        fileDataLenUse = Math.Min(fileDataLen, (int)(infoTorr.PieceSize - blockSHA1_TrLength));
                                        blockSHA1.TransformBlock(fileData, fileDataIdx, fileDataLenUse, blockSHA1_OBuff, 0);
                                        blockSHA1_TrLength += fileDataLenUse;
                                        fileDataIdx += fileDataLenUse;
                                        fileDataLen -= fileDataLenUse;
                                        if (blockSHA1_TrLength == infoTorr.PieceSize)
                                        {
                                            blockSHA1.TransformFinalBlock(new byte[] { }, 0, 0);
                                            blockResult[blockIdx] = ArrayEquals(blockSHA1.Hash, 0, 20, infoTorr.Pieces, 20 * blockIdx, 20) ? (byte)1 : (byte)2;
                                            blockSHA1.Clear();
                                            blockSHA1 = null;
                                            blockSHA1_OBuff = null;
                                            blockSHA1_TrLength = 0;
                                            blockIdx++;
                                            this.Invoke(new Action(UpdateProgressBar));
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), ex.Message);
                            }
                            tFileIdx++;
                        }
                        if (_ProcCancel == false && blockSHA1 != null)
                        {
                            blockSHA1.TransformFinalBlock(new byte[] { }, 0, 0);
                            blockResult[blockIdx] = ArrayEquals(blockSHA1.Hash, 0, 20, infoTorr.Pieces, 20 * blockIdx, 20) ? (byte)1 : (byte)2;
                            blockSHA1.Clear();
                            blockSHA1 = null;
                            blockSHA1_OBuff = null;
                            blockSHA1_TrLength = 0;
                            blockIdx++;
                            this.Invoke(new Action(UpdateProgressBar));
                        }
                    }
                }
                else
                    this.Invoke(new Action<string>(errMsg => { MessageBox.Show(this, errMsg, "", MessageBoxButtons.OK, MessageBoxIcon.Error); _ProcCancel = true; btnCheck.Enabled = false; }), "种子文件格式有误！");
            }

            if (!_ProcCancel)
            {
                int[] cnt = new int[] { 0, 0, 0 };
                blockIdx = blockIndexBg;
                while (blockIdx < blockIndexEd + 1)
                {
                    cnt[blockResult[blockIdx]]++;
                    blockIdx++;
                }
                fileMD5.TransformFinalBlock(new byte[] { }, 0, 0);
                fileSHA1.TransformFinalBlock(new byte[] { }, 0, 0);
                fileSHA256.TransformFinalBlock(new byte[] { }, 0, 0);
                this.Invoke(new Action<string, string, string, int, int, int>((hashMD5, hashSHA1, hashSHA256, cnt0, cnt1, cnt2) =>
                  {
                      tbxMD5.Text = hashMD5; tbxSHA1.Text = hashSHA1; tbxSHA256.Text = hashSHA256;
                      var tbxClr = cnt2 != 0 ? brush2.Color : (cnt0 == 0 ? brush1.Color : brush0.Color);
                      tbxMD5.ForeColor = tbxClr; tbxSHA1.ForeColor = tbxClr; tbxSHA256.ForeColor = tbxClr;
                      if (cnt2 != 0)
                          MessageBox.Show(this, $"文件共{cnt0 + cnt1 + cnt2}个区块，有{cnt2}个区块数据校验错误{(cnt0 == 0 ? "" : $"，{cnt0}个区块数据未校验")}！", "", MessageBoxButtons.OK, MessageBoxIcon.Error);
                      else if (cnt0 != 0)
                          MessageBox.Show(this, $"文件共{cnt0 + cnt1 + cnt2}个区块，有{cnt0}个区块数据未校验！", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                      else
                          MessageBox.Show(this, $"文件校验通过！", "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                  }), BitConverter.ToString(fileMD5.Hash).Replace("-", ""), BitConverter.ToString(fileSHA1.Hash).Replace("-", ""), BitConverter.ToString(fileSHA256.Hash).Replace("-", ""), cnt[0], cnt[1], cnt[2]);
            }

            this.Invoke(new Action(() =>
            {
                btnTorrent.Enabled = true;
                btnFile.Enabled = true;
                btnCheck.Text = "校验";
                btnCheck.Enabled = true;
                _ProcCancel = true;
            }));

        }


        private string _SelectFile(string fName, long fSize)
        {
            string retVal;
            do
            {
                var dlg = new OpenFileDialog();
                dlg.Filter = "所有文件|*.*";
                dlg.Title = $"打开\"{fName}\" ({fSize}字节)";
                retVal = dlg.ShowDialog(this) == DialogResult.OK ? dlg.FileName : null;
                if (string.IsNullOrEmpty(retVal))
                {
                    if (MessageBox.Show(this, "取消校验此区块数据？", "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        break;
                }
                else
                    try
                    {
                        if (new System.IO.FileInfo(retVal).Length != fSize)
                            throw new Exception($"文件名：{fName}，大小：{fSize} 字节，选择的文件大小不匹配！");
                    }
                    catch (Exception ex)
                    {
                        retVal = null;
                        MessageBox.Show(this, ex.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
            } while (string.IsNullOrEmpty(retVal));
            return retVal;
        }


    }
}
