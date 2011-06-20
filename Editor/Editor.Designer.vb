﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Editor
    Inherits System.Windows.Forms.UserControl

    'UserControl1 overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.splitMain = New System.Windows.Forms.SplitContainer()
        Me.ctlTree = New AxeSoftware.Quest.EditorControls.WFEditorTree()
        Me.pnlContent = New System.Windows.Forms.Panel()
        Me.pnlHeader = New System.Windows.Forms.Panel()
        Me.ctlBanner = New AxeSoftware.Quest.AlertBanner()
        Me.lblHeader = New System.Windows.Forms.Label()
        Me.ctlSaveFile = New System.Windows.Forms.SaveFileDialog()
        Me.ctlTextEditor = New AxeSoftware.Quest.TextEditorControl()
        Me.ctlToolbar = New AxeSoftware.Quest.MainToolbar()
        Me.ctlPublishFile = New System.Windows.Forms.SaveFileDialog()
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.splitMain.Panel1.SuspendLayout()
        Me.splitMain.Panel2.SuspendLayout()
        Me.splitMain.SuspendLayout()
        Me.pnlHeader.SuspendLayout()
        Me.SuspendLayout()
        '
        'splitMain
        '
        Me.splitMain.Dock = System.Windows.Forms.DockStyle.Fill
        Me.splitMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel1
        Me.splitMain.Location = New System.Drawing.Point(0, 25)
        Me.splitMain.Name = "splitMain"
        '
        'splitMain.Panel1
        '
        Me.splitMain.Panel1.Controls.Add(Me.ctlTree)
        '
        'splitMain.Panel2
        '
        Me.splitMain.Panel2.Controls.Add(Me.pnlContent)
        Me.splitMain.Panel2.Controls.Add(Me.pnlHeader)
        Me.splitMain.Size = New System.Drawing.Size(618, 304)
        Me.splitMain.SplitterDistance = 206
        Me.splitMain.TabIndex = 0
        '
        'ctlTree
        '
        Me.ctlTree.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ctlTree.IncludeRootLevelInSearchResults = True
        Me.ctlTree.Location = New System.Drawing.Point(0, 0)
        Me.ctlTree.Name = "ctlTree"
        Me.ctlTree.ShowFilterBar = True
        Me.ctlTree.Size = New System.Drawing.Size(206, 304)
        Me.ctlTree.TabIndex = 0
        '
        'pnlContent
        '
        Me.pnlContent.Dock = System.Windows.Forms.DockStyle.Fill
        Me.pnlContent.Location = New System.Drawing.Point(0, 41)
        Me.pnlContent.Name = "pnlContent"
        Me.pnlContent.Size = New System.Drawing.Size(408, 263)
        Me.pnlContent.TabIndex = 1
        '
        'pnlHeader
        '
        Me.pnlHeader.Controls.Add(Me.ctlBanner)
        Me.pnlHeader.Controls.Add(Me.lblHeader)
        Me.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.pnlHeader.Location = New System.Drawing.Point(0, 0)
        Me.pnlHeader.Name = "pnlHeader"
        Me.pnlHeader.Size = New System.Drawing.Size(408, 41)
        Me.pnlHeader.TabIndex = 0
        '
        'ctlBanner
        '
        Me.ctlBanner.AlertText = "Text"
        Me.ctlBanner.BackColor = System.Drawing.Color.FromArgb(CType(CType(255, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(192, Byte), Integer))
        Me.ctlBanner.ButtonText = "Action"
        Me.ctlBanner.Dock = System.Windows.Forms.DockStyle.Top
        Me.ctlBanner.Location = New System.Drawing.Point(0, 16)
        Me.ctlBanner.Name = "ctlBanner"
        Me.ctlBanner.Size = New System.Drawing.Size(408, 23)
        Me.ctlBanner.TabIndex = 1
        '
        'lblHeader
        '
        Me.lblHeader.Dock = System.Windows.Forms.DockStyle.Top
        Me.lblHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblHeader.Location = New System.Drawing.Point(0, 0)
        Me.lblHeader.Name = "lblHeader"
        Me.lblHeader.Size = New System.Drawing.Size(408, 16)
        Me.lblHeader.TabIndex = 0
        '
        'ctlSaveFile
        '
        Me.ctlSaveFile.DefaultExt = "aslx"
        Me.ctlSaveFile.Filter = "Quest Games|*.aslx|All files|*.*"
        '
        'ctlTextEditor
        '
        Me.ctlTextEditor.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ctlTextEditor.EditText = ""
        Me.ctlTextEditor.Location = New System.Drawing.Point(0, 25)
        Me.ctlTextEditor.Name = "ctlTextEditor"
        Me.ctlTextEditor.Size = New System.Drawing.Size(618, 304)
        Me.ctlTextEditor.TabIndex = 3
        Me.ctlTextEditor.Visible = False
        '
        'ctlToolbar
        '
        Me.ctlToolbar.CodeView = False
        Me.ctlToolbar.Dock = System.Windows.Forms.DockStyle.Top
        Me.ctlToolbar.Location = New System.Drawing.Point(0, 0)
        Me.ctlToolbar.Name = "ctlToolbar"
        Me.ctlToolbar.RedoButtonEnabled = False
        Me.ctlToolbar.Size = New System.Drawing.Size(618, 25)
        Me.ctlToolbar.TabIndex = 2
        Me.ctlToolbar.UndoButtonEnabled = False
        '
        'ctlPublishFile
        '
        Me.ctlPublishFile.DefaultExt = "quest"
        Me.ctlPublishFile.Filter = "Quest Games|*.quest|All files|*.*"
        '
        'Editor
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Controls.Add(Me.splitMain)
        Me.Controls.Add(Me.ctlTextEditor)
        Me.Controls.Add(Me.ctlToolbar)
        Me.Name = "Editor"
        Me.Size = New System.Drawing.Size(618, 329)
        Me.splitMain.Panel1.ResumeLayout(False)
        Me.splitMain.Panel2.ResumeLayout(False)
        CType(Me.splitMain, System.ComponentModel.ISupportInitialize).EndInit()
        Me.splitMain.ResumeLayout(False)
        Me.pnlHeader.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents splitMain As System.Windows.Forms.SplitContainer
    Friend WithEvents ctlTree As AxeSoftware.Quest.EditorControls.WFEditorTree
    Friend WithEvents ctlToolbar As AxeSoftware.Quest.MainToolbar
    Private WithEvents ctlSaveFile As System.Windows.Forms.SaveFileDialog
    Friend WithEvents pnlHeader As System.Windows.Forms.Panel
    Friend WithEvents pnlContent As System.Windows.Forms.Panel
    Friend WithEvents lblHeader As System.Windows.Forms.Label
    Friend WithEvents ctlTextEditor As AxeSoftware.Quest.TextEditorControl
    Friend WithEvents ctlBanner As AxeSoftware.Quest.AlertBanner
    Private WithEvents ctlPublishFile As System.Windows.Forms.SaveFileDialog

End Class
