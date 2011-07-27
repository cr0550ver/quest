﻿Imports System.Xml
Imports System.Threading

Public Class Player

    Implements IPlayerHelperUI

    Private m_htmlHelper As PlayerHelper
    Private m_panesVisible As Boolean
    Private WithEvents m_game As IASL
    Private WithEvents m_gameTimer As IASLTimer
    Private m_gameDebug As IASLDebug
    Private m_initialised As Boolean
    Private m_gameReady As Boolean
    Private m_history As New List(Of String)
    Private m_historyPoint As Integer
    Private m_gameName As String
    Private WithEvents m_debugger As Debugger
    Private m_loaded As Boolean = False
    Private m_splitHelpers As New List(Of AxeSoftware.Utility.SplitterHelper)
    Private m_menu As AxeSoftware.Quest.Controls.Menu = Nothing
    Private m_saveFile As String
    Private m_waiting As Boolean = False
    Private m_speech As New System.Speech.Synthesis.SpeechSynthesizer
    Private m_loopSound As Boolean = False
    Private m_waitingForSoundToFinish As Boolean = False
    Private m_soundPlaying As Boolean = False
    Private m_destroyed As Boolean = False
    Private WithEvents m_mediaPlayer As New System.Windows.Media.MediaPlayer
    Private m_htmlPlayerReadyFunction As Action
    Private m_tickCount As Integer
    Private m_sendNextTickEventAfter As Integer
    Private m_pausing As Boolean
    Private WithEvents m_walkthroughRunner As WalkthroughRunner
    Private m_postLaunchAction As Action
    Private m_recordWalkthrough As String
    Private m_recordedWalkthrough As List(Of String)

    Public Event Quit()
    Public Event AddToRecent(filename As String, name As String)
    Public Event GameNameSet(name As String)
    Public Event ShortcutKeyPressed(keys As System.Windows.Forms.Keys)
    Public Event ExitFullScreen()
    Public Event RecordedWalkthrough(name As String, steps As List(Of String))

    Public Sub New()
        ' This call is required by the Windows Form Designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        PanesVisible = True
        Reset()

        lstPlacesObjects.IgnoreNames = ctlCompass.CompassDirections

        m_splitHelpers.Add(New AxeSoftware.Utility.SplitterHelper(splitMain, "Quest", "MainSplitter"))
        m_splitHelpers.Add(New AxeSoftware.Utility.SplitterHelper(splitPane, "Quest", "PaneSplitter"))

    End Sub

    Public Sub SetMenu(menu As AxeSoftware.Quest.Controls.Menu)
        m_menu = menu

        menu.AddMenuClickHandler("debugger", AddressOf DebuggerMenuClick)
        menu.AddMenuClickHandler("walkthrough", AddressOf RunWalkthrough)
        menu.AddMenuClickHandler("undo", AddressOf Undo)
        menu.AddMenuClickHandler("selectall", AddressOf SelectAll)
        menu.SetShortcut("selectall", Keys.Control Or Keys.A)
        menu.AddMenuClickHandler("copy", AddressOf Copy)
        menu.SetShortcut("copy", Keys.Control Or Keys.C)
        menu.AddMenuClickHandler("save", AddressOf Save)
        menu.AddMenuClickHandler("saveas", AddressOf SaveAs)
        menu.AddMenuClickHandler("stop", AddressOf StopGame)
    End Sub

    Private Sub DebuggerMenuClick()
        ShowDebugger(Not m_menu.MenuChecked("debugger"))
    End Sub

    Public Sub Initialise(ByRef game As IASL)
        SetPanesVisible(True)
        SetCommandVisible(True)
        SetLocationVisible(True)
        SetStatusText("")
        LocationUpdated("")
        cmdFullScreen.Visible = False
        m_menu.ClearWindowMenu()
        m_menu.MenuEnabled("copy") = True
        m_game = game
        m_gameDebug = TryCast(game, IASLDebug)
        m_gameTimer = TryCast(m_game, IASLTimer)
        m_gameReady = True
        txtCommand.Text = ""
        SetEnabledState(True)
        m_htmlHelper = New PlayerHelper(m_game, Me)
        m_htmlPlayerReadyFunction = AddressOf FinishInitialise

        ' we don't finish initialising the game until the webbrowser's DocumentCompleted fires
        ' - it's not in a ready state before then.
        ctlPlayerHtml.Setup()
    End Sub

    Private Sub FinishInitialise()

        If m_game.Initialise(Me) Then

            AddToRecentList()
            m_menu.MenuEnabled("walkthrough") = m_gameDebug IsNot Nothing AndAlso m_gameDebug.Walkthroughs IsNot Nothing AndAlso m_gameDebug.Walkthroughs.Walkthroughs.Count > 0
            m_menu.MenuEnabled("debugger") = m_gameDebug IsNot Nothing AndAlso m_gameDebug.DebugEnabled

            ' If we have external JavaScript files, we need to rebuild the HTML page source and
            ' reload it. Then, only when the page has finished loading, begin the game.

            Dim scripts As IEnumerable(Of String) = m_game.GetExternalScripts
            If scripts IsNot Nothing AndAlso scripts.Count > 0 Then
                ' Generate the new HTML and wait for Ready event

                m_htmlPlayerReadyFunction = AddressOf BeginGame
                ctlPlayerHtml.InitialiseScripts(scripts)
            Else
                BeginGame()
            End If

        Else
            WriteLine("<b>Failed to load game.</b>")
            If (m_game.Errors.Count > 0) Then
                WriteLine("The following errors occurred:")
                For Each loadError As String In m_game.Errors
                    WriteLine(loadError)
                Next
            End If
            GameFinished()
        End If

    End Sub

    Private Sub BeginGame()
        m_initialised = True
        m_game.Begin()
        ClearBuffer()
        txtCommand.Focus()
        ctlPlayerHtml.DisableNavigation()
        If m_postLaunchAction IsNot Nothing Then
            m_postLaunchAction.Invoke()
            m_postLaunchAction = Nothing
        End If
    End Sub

    Private Sub ClearBuffer()
        If Not Me.IsHandleCreated Then Return
        WriteHTML(m_htmlHelper.ClearBuffer())
        BeginInvoke(Sub() ctlPlayerHtml.ClearBuffer())
    End Sub

    Public Sub Reset()
        If Not m_game Is Nothing Then m_game.Finish()
        m_initialised = False
        m_gameReady = False
        m_gameName = ""
        ShowDebugger(False)
        m_debugger = Nothing
        lstInventory.Clear()
        lstPlacesObjects.Clear()
        If Not m_menu Is Nothing Then
            m_menu.MenuEnabled("walkthrough") = False
            m_menu.MenuEnabled("debugger") = False
        End If
    End Sub

    Public Property PanesVisible() As Boolean
        Get
            Return m_panesVisible
        End Get
        Set(value As Boolean)
            m_panesVisible = value
            splitMain.Panel2Collapsed = Not m_panesVisible
            If m_panesVisible Then
                cmdPanes.Text = ">"
            Else
                cmdPanes.Text = "<"
            End If
        End Set
    End Property

    Private Sub txtCommand_KeyDown(sender As Object, e As System.Windows.Forms.KeyEventArgs) Handles txtCommand.KeyDown
        Select Case e.KeyCode
            Case Keys.Up
                SetHistoryPoint(-1)
                e.Handled = True
            Case Keys.Down
                SetHistoryPoint(1)
                e.Handled = True
            Case Keys.Escape
                txtCommand.Text = ""
                SetHistoryPoint(0)
                e.SuppressKeyPress = True
                e.Handled = True
            Case Keys.Enter
                e.SuppressKeyPress = True
                e.Handled = True
                ' Use a timer to trigger the call to EnterText, otherwise if we handle an event
                ' which shows the Menu form, we get a "ding" as this sub hasn't returned yet.
                tmrTimer.Tag = Nothing
                tmrTimer.Enabled = True
        End Select
    End Sub

    Private Sub SetHistoryPoint(offset As Integer)
        If offset = 0 Then
            m_historyPoint = m_history.Count
        Else
            If m_history.Count > 0 Then
                m_historyPoint = m_historyPoint + offset

                If m_historyPoint < 0 Then m_historyPoint = m_history.Count - 1
                If m_historyPoint >= m_history.Count Then m_historyPoint = 0

                txtCommand.Text = m_history.Item(m_historyPoint)
                txtCommand.SelectionStart = txtCommand.Text.Length
            End If
        End If
    End Sub

    Private Sub cmdGo_Click(sender As System.Object, e As System.EventArgs) Handles cmdGo.Click
        If m_waiting Then
            m_waiting = False
            Exit Sub
        End If
        EnterText()
    End Sub

    Private Sub cmdPanes_Click(sender As System.Object, e As System.EventArgs) Handles cmdPanes.Click
        PanesVisible = Not PanesVisible
    End Sub

    Private Sub EnterText()
        If m_pausing Then Return
        If txtCommand.Text.Length > 0 Then
            m_history.Add(txtCommand.Text)
            SetHistoryPoint(0)
            RunCommand(txtCommand.Text)
        End If
    End Sub

    Private Sub RunCommand(command As String)

        If Not m_initialised Then Exit Sub

        If RecordWalkthrough IsNot Nothing Then
            m_recordedWalkthrough.Add(command)
        End If

        txtCommand.Text = ""

        Try
            If m_gameTimer IsNot Nothing Then
                m_gameTimer.SendCommand(command, GetTickCountAndStopTimer())
            Else
                m_game.SendCommand(command)
            End If
            ClearBuffer()
        Catch ex As Exception
            WriteLine(String.Format("Error running command '{0}':<br>{1}", command, ex.Message))
        End Try

        txtCommand.Focus()

    End Sub

    Private Sub ctlCompass_RunCommand(command As String) Handles ctlCompass.RunCommand
        RunCommand(command)
    End Sub

    Private Sub m_game_Finished() Handles m_game.Finished
        If Not Me.IsHandleCreated Then Return
        BeginInvoke(Sub() GameFinished())
    End Sub

    Private Sub GameFinished()
        If Not m_initialised Then Return
        m_initialised = False
        tmrTick.Enabled = False
        SetEnabledState(False)
        StopSound()
        If Me.IsHandleCreated Then
            BeginInvoke(Sub() ctlPlayerHtml.Finished())
        End If
        ClearBuffer()
        If RecordWalkthrough IsNot Nothing Then
            RaiseEvent RecordedWalkthrough(RecordWalkthrough, m_recordedWalkthrough)
        End If
        RecordWalkthrough = Nothing
    End Sub

    Private Sub m_game_LogError(errorMessage As String) Handles m_game.LogError
        If Not Me.IsHandleCreated Then Return
        BeginInvoke(Sub()
                        WriteLine("<output><b>Sorry, an error occurred.</b></output>")
                        WriteLine("<output>" + errorMessage + "</output>")
                        ClearBuffer()
                    End Sub)
    End Sub

    Private Sub AddToRecentList()
        If Not String.IsNullOrEmpty(m_game.SaveFilename) Then
            RaiseEvent AddToRecent(m_game.SaveFilename, m_gameName + " (Saved)")
            m_saveFile = m_game.SaveFilename
        ElseIf Not m_game.OriginalFilename Is Nothing Then
            RaiseEvent AddToRecent(m_game.OriginalFilename, m_gameName)
        ElseIf Not m_game.Filename Is Nothing Then
            RaiseEvent AddToRecent(m_game.Filename, m_gameName)
        End If
    End Sub

    Private Sub tmrTimer_Tick(sender As Object, e As System.EventArgs) Handles tmrTimer.Tick
        tmrTimer.Enabled = False
        EnterText()
    End Sub

    Private Sub SetGameName(name As String)
        m_gameName = name
        AddToRecentList()
        RaiseEvent GameNameSet(name)
    End Sub

    Private Sub m_game_UpdateList(listType As AxeSoftware.Quest.ListType, items As System.Collections.Generic.List(Of AxeSoftware.Quest.ListData)) Handles m_game.UpdateList
        BeginInvoke(Sub() UpdateList(listType, items))
    End Sub

    Private Sub UpdateList(listType As AxeSoftware.Quest.ListType, items As System.Collections.Generic.List(Of AxeSoftware.Quest.ListData))
        ' Keep the IASL interface atomic, so we transmit lists of places separately to lists of objects.
        ' We can merge them when we receive them here, and then pass the merged list to the ElementList with some
        ' kind of flag so it knows what's a place and what's an object.

        Select Case listType
            Case AxeSoftware.Quest.ListType.ObjectsList
                lstPlacesObjects.Items = items
            Case AxeSoftware.Quest.ListType.InventoryList
                lstInventory.Items = items
            Case AxeSoftware.Quest.ListType.ExitsList
                ctlCompass.SetAvailableExits(items)
        End Select
    End Sub

    Private Sub lstPlacesObjects_SendCommand(command As String) Handles lstPlacesObjects.SendCommand
        RunCommand(command)
    End Sub

    Private Sub lstInventory_SendCommand(command As String) Handles lstInventory.SendCommand
        RunCommand(command)
    End Sub

    Public ReadOnly Property IsGameInProgress() As Boolean
        Get
            Return m_initialised
        End Get
    End Property

    Public Sub ShowDebugger(show As Boolean)
        If show Then
            If m_debugger Is Nothing Then
                m_debugger = New Debugger
                m_debugger.Game = DirectCast(m_game, IASLDebug)
            End If
            m_debugger.Show()
            m_debugger.LoadSplitterPositions()
        Else
            If Not m_debugger Is Nothing Then
                m_debugger.Hide()
            End If
        End If
    End Sub

    Private Sub m_debugger_VisibleChanged(sender As Object, e As System.EventArgs) Handles m_debugger.VisibleChanged
        m_menu.MenuChecked("debugger") = m_debugger.Visible
    End Sub

    Private Sub RunWalkthrough()
        ' Eventually we want to pop up a debugging panel on the right of the screen where we can select
        ' walkthroughs, step through etc.

        Dim walkThrough As String = ChooseWalkthrough()
        If walkThrough Is Nothing Then Return

        RunWalkthrough(walkThrough)
    End Sub

    Public Sub RunWalkthrough(name As String)
        m_walkthroughRunner = New WalkthroughRunner(m_gameDebug, name)

        Dim runnerThread As New Thread(Sub() WalkthroughRunner())
        runnerThread.Start()
    End Sub

    Private Sub WalkthroughRunner()
        Try
            m_walkthroughRunner.Run()

        Catch ex As Exception
            BeginInvoke(Sub() WriteLine(String.Format("Error - walkthrough halted:<br>{0}", ex.Message)))

        Finally
            m_walkthroughRunner = Nothing
        End Try
    End Sub

    Private Function ChooseWalkthrough() As String
        If m_gameDebug.Walkthroughs.Walkthroughs.Count = 0 Then
            Return Nothing
        End If

        If m_gameDebug.Walkthroughs.Walkthroughs.Count = 1 Then
            Return m_gameDebug.Walkthroughs.Walkthroughs.First.Key
        End If

        Dim menuForm As New Menu()
        Dim menuOptions As New Dictionary(Of String, String)

        For Each item In m_gameDebug.Walkthroughs.Walkthroughs
            menuOptions.Add(item.Key, item.Key)
        Next

        menuForm.Caption = "Please choose a walkthrough to run:"
        menuForm.Options = menuOptions
        menuForm.AllowCancel = True
        menuForm.ShowDialog()

        Return menuForm.SelectedItem
    End Function

    Private Sub SetBackground(colour As String) Implements IPlayer.SetBackground
        BeginInvoke(Sub() ctlPlayerHtml.SetBackground(colour))
    End Sub

    Private Sub RunScript(data As String) Implements IPlayer.RunScript
        ClearBuffer()
        BeginInvoke(Sub()
                        Dim functionName As String = ""
                        Dim dataList As List(Of String)
                        Dim args As New List(Of String)

                        dataList = New List(Of String)(data.Split(CChar(";")))

                        For Each value As String In dataList
                            If functionName.Length = 0 Then
                                functionName = value
                            Else
                                args.Add(value.Trim())
                            End If
                        Next

                        ctlPlayerHtml.InvokeScript(functionName, args.ToArray())
                    End Sub)
    End Sub

    Private Sub Undo()
        RunCommand("undo")
    End Sub

    Private Sub Copy()
        ctlPlayerHtml.Copy()
    End Sub

    Private Sub SelectAll()
        ctlPlayerHtml.SelectAll()
    End Sub

    Private Sub SetEnabledState(enabled As Boolean)
        txtCommand.Enabled = enabled
        ctlCompass.Enabled = enabled
        cmdGo.Enabled = enabled
        lstInventory.Enabled = enabled
        lstPlacesObjects.Enabled = enabled
    End Sub

    Public Sub RestoreSplitterPositions()
        For Each splitHelper As AxeSoftware.Utility.SplitterHelper In m_splitHelpers
            splitHelper.LoadSplitterPositions()
        Next
    End Sub

    Private Sub Save()
        If Len(m_saveFile) = 0 Then
            SaveAs()
        Else
            Save(m_saveFile)
        End If
    End Sub

    Private Sub SaveAs()
        ctlSaveFile.DefaultExt = m_game.SaveExtension
        ctlSaveFile.Filter = "Saved Games|*." + m_game.SaveExtension + "|All files|*.*"
        ctlSaveFile.FileName = m_saveFile
        If ctlSaveFile.ShowDialog() = DialogResult.OK Then
            m_saveFile = ctlSaveFile.FileName
            Save(m_saveFile)
        End If
    End Sub

    Private Sub Save(filename As String)
        Try
            m_game.Save(filename)
            RaiseEvent AddToRecent(filename, m_gameName + " (Saved)")
            WriteLine("")
            WriteLine("Saved: " + filename)
            ClearBuffer()
        Catch ex As Exception
            MsgBox("Unable to save the file due to the following error:" + Environment.NewLine + Environment.NewLine + ex.Message, MsgBoxStyle.Critical)
        End Try
    End Sub

    Public Sub ShowMenu(menuData As MenuData) Implements IPlayer.ShowMenu
        If m_walkthroughRunner IsNot Nothing Then
            m_walkthroughRunner.ShowMenu(menuData)
        Else
            BeginInvoke(Sub()
                            Dim menuForm As Menu
                            menuForm = New Menu()

                            menuForm.Caption = menuData.Caption
                            menuForm.Options = menuData.Options
                            menuForm.AllowCancel = menuData.AllowCancel
                            menuForm.ShowDialog()

                            If RecordWalkthrough IsNot Nothing Then
                                m_recordedWalkthrough.Add("menu:" + menuForm.SelectedItem)
                            End If

                            Dim runnerThread As New System.Threading.Thread(New System.Threading.ParameterizedThreadStart(AddressOf SetMenuResponseInNewThread))
                            runnerThread.Start(menuForm.SelectedItem)
                        End Sub
            )
        End If
    End Sub

    Private Sub SetMenuResponseInNewThread(response As Object)
        m_game.SetMenuResponse(DirectCast(response, String))
        ClearBuffer()
    End Sub

    Public Sub DoWait() Implements IPlayer.DoWait
        BeginInvoke(Sub() BeginWait())
    End Sub

    Private Sub BeginWait()
        m_waiting = True
        Do
            Threading.Thread.Sleep(100)
            Application.DoEvents()
        Loop Until Not m_waiting Or Not m_initialised Or Not Me.IsHandleCreated
        m_game.FinishWait()
        ClearBuffer()
    End Sub

    Public Sub ShowQuestion(caption As String) Implements IPlayer.ShowQuestion
        BeginInvoke(Sub()
                        Dim result As Boolean = (MsgBox(caption, MsgBoxStyle.Question Or MsgBoxStyle.YesNo, m_gameName) = MsgBoxResult.Yes)
                        m_game.SetQuestionResponse(result)
                        ClearBuffer()
                    End Sub
        )
    End Sub

    Public Function KeyPressed() As Boolean
        If m_waiting Then
            m_waiting = False
            Return True
        End If

        Return False
    End Function

    Private Sub wbOutput_PreviewKeyDown(sender As Object, e As System.Windows.Forms.PreviewKeyDownEventArgs)
        KeyPressed()
    End Sub

    Private Sub SetPanesVisible(data As String) Implements IPlayer.SetPanesVisible
        BeginInvoke(Sub()
                        Select Case data
                            Case "on"
                                PanesVisible = True
                                cmdPanes.Visible = True
                            Case "off"
                                PanesVisible = False
                                cmdPanes.Visible = True
                            Case "disabled"
                                PanesVisible = False
                                cmdPanes.Visible = False
                        End Select

                        If cmdPanes.Visible Then
                            lblBanner.Width = cmdPanes.Left - 1
                        Else
                            lblBanner.Width = ctlPlayerHtml.Width
                        End If
                    End Sub)
    End Sub

    Private Sub ShowPicture(filename As String) Implements IPlayer.ShowPicture
        ClearBuffer()
        BeginInvoke(Sub() ctlPlayerHtml.ShowPicture(filename))
    End Sub

    Private Sub PlaySound(filename As String, synchronous As Boolean, looped As Boolean) Implements IPlayer.PlaySound
        BeginInvoke(Sub()
                        If synchronous And looped Then
                            Throw New Exception("Can't play sound that is both synchronous and looped")
                        End If
                        m_loopSound = looped
                        m_soundPlaying = True

                        m_mediaPlayer.Open(New System.Uri(filename))
                        m_mediaPlayer.Play()

                        m_waitingForSoundToFinish = synchronous
                    End Sub
        )
    End Sub

    Private Sub StopSound() Implements IPlayer.StopSound
        If m_destroyed Then Exit Sub
        BeginInvoke(Sub()
                        m_loopSound = False
                        m_mediaPlayer.Stop()
                    End Sub
        )
    End Sub

    Private Sub m_mediaPlayer_MediaEnded(sender As Object, e As System.EventArgs) Handles m_mediaPlayer.MediaEnded
        m_soundPlaying = False
        If m_waitingForSoundToFinish Then
            m_waitingForSoundToFinish = False
            m_game.FinishWait()
            ClearBuffer()
        End If
        If m_loopSound Then
            m_mediaPlayer.Position = New TimeSpan(0)
            m_mediaPlayer.Play()
        End If
    End Sub

    Private Sub Speak(text As String) Implements IPlayer.Speak
        BeginInvoke(Sub() m_speech.Speak(text))
    End Sub

    Public Sub SetWindowMenu(menuData As MenuData) Implements IPlayer.SetWindowMenu
        BeginInvoke(Sub()
                        m_menu.CreateWindowMenu(menuData.Caption, menuData.Options, AddressOf WindowMenuClicked)
                    End Sub
        )
    End Sub

    Private Sub WindowMenuClicked(command As String)
        RunCommand(command)
    End Sub

    Public Function GetNewGameFile(originalFilename As String, extensions As String) As String Implements IPlayer.GetNewGameFile
        If MsgBox(String.Format("The game file {0} does not exist.{1}Would you like to find the file yourself?", originalFilename, vbCrLf + vbCrLf), MsgBoxStyle.Question Or MsgBoxStyle.YesNo) = MsgBoxResult.Yes Then
            ctlOpenFile.Filter = "Game files|" + extensions
            ctlOpenFile.ShowDialog()
            Return ctlOpenFile.FileName
        Else
            Return ""
        End If
    End Function

    Public Sub WindowClosing()
        If Not m_game Is Nothing Then
            m_game.Finish()
        End If
    End Sub

    Private Sub tmrTick_Tick(sender As System.Object, e As System.EventArgs) Handles tmrTick.Tick
        If Not m_gameTimer Is Nothing Then
            m_tickCount += 1
            If m_sendNextTickEventAfter > 0 AndAlso m_tickCount >= m_sendNextTickEventAfter Then
                m_gameTimer.Tick(GetTickCountAndStopTimer())
            End If
        End If
    End Sub

    Private Function GetTickCountAndStopTimer() As Integer
        tmrTick.Enabled = False
        Return m_tickCount
    End Function

    Private Sub Player_HandleDestroyed(sender As Object, e As System.EventArgs) Handles Me.HandleDestroyed
        m_destroyed = True
    End Sub

    Public Sub WriteHTML(html As String) Implements IPlayer.WriteHTML
        If Not Me.IsHandleCreated Then Return
        BeginInvoke(Sub() ctlPlayerHtml.WriteText(html))
    End Sub

    Public Sub LocationUpdated(location As String) Implements IPlayer.LocationUpdated
        BeginInvoke(Sub() lblBanner.Text = location)
    End Sub

    Public Sub UpdateGameName(name As String) Implements IPlayer.UpdateGameName
        BeginInvoke(Sub() SetGameName(name))
    End Sub

    Public Sub ClearScreen() Implements IPlayer.ClearScreen
        ClearBuffer()
        BeginInvoke(Sub() ctlPlayerHtml.Clear())
    End Sub

    Public Sub SetStatusText(text As String) Implements IPlayer.SetStatusText
        BeginInvoke(Sub() lstInventory.Status = text)
    End Sub

    Public Sub DoQuit() Implements IPlayer.Quit
        BeginInvoke(Sub()
                        GameFinished()
                        RaiseEvent Quit()
                    End Sub)
    End Sub

    Public Sub SetForeground(colour As String) Implements IPlayer.SetForeground
        m_htmlHelper.SetForeground(colour)
    End Sub

    Public Sub SetFont(name As String) Implements IPlayer.SetFont
        m_htmlHelper.SetFont(name)
    End Sub

    Public Sub SetFontSize(size As String) Implements IPlayer.SetFontSize
        m_htmlHelper.SetFontSize(size)
    End Sub

    Public Sub RequestLoad() Implements IPlayer.RequestLoad
        ' TO DO: Raise event
    End Sub

    Public Sub RequestRestart() Implements IPlayer.RequestRestart
        ' TO DO: Raise event
    End Sub

    Public Sub RequestSave() Implements IPlayer.RequestSave
        BeginInvoke(Sub() Save())
    End Sub

    Public Sub SetLinkForeground(colour As String) Implements IPlayer.SetLinkForeground
        m_htmlHelper.SetLinkForeground(colour)
    End Sub

    Private Sub WriteLine(text As String)
        ctlPlayerHtml.WriteLine(text)
    End Sub

    Private Sub ctlPlayerHtml_CommandRequested(command As String) Handles ctlPlayerHtml.CommandRequested
        RunCommand(command)
    End Sub

    Private Sub ctlPlayerHtml_Ready() Handles ctlPlayerHtml.Ready
        ' We need this DocumentCompleted event to have finished before trying to output text to the webbrowser control.
        'Dim runnerThread As New Thread(New ThreadStart(AddressOf TryInitialise))
        'runnerThread.Start()

        BeginInvoke(Sub()
                        m_htmlPlayerReadyFunction.Invoke()
                    End Sub)
    End Sub

    Private Sub ctlPlayerHtml_SendEvent(eventName As String, param As String) Handles ctlPlayerHtml.SendEvent
        m_game.SendEvent(eventName, param)
        ClearBuffer()
    End Sub

    Public Sub BindMenu(linkid As String, verbs As String, text As String) Implements IPlayerHelperUI.BindMenu
        BeginInvoke(Sub() ctlPlayerHtml.BindMenu(linkid, verbs, text))
    End Sub

    Public Sub OutputText(text As String) Implements IPlayerHelperUI.OutputText
        BeginInvoke(Sub() ctlPlayerHtml.WriteText(text))
    End Sub

    Public Sub SetAlignment(alignment As String) Implements IPlayerHelperUI.SetAlignment
        ClearBuffer()
        BeginInvoke(Sub() ctlPlayerHtml.SetAlignment(alignment))
    End Sub

    Public Sub DoHide(element As String) Implements IPlayer.Hide
        BeginInvoke(Sub() GetInterfaceVisibilitySetter(element).Invoke(False))
    End Sub

    Public Sub DoShow(element As String) Implements IPlayer.Show
        BeginInvoke(Sub() GetInterfaceVisibilitySetter(element).Invoke(True))
    End Sub

    Private Function GetInterfaceVisibilitySetter(element As String) As Action(Of Boolean)
        Select Case element
            Case "Panes"
                Return AddressOf SetPanesVisible
            Case "Location"
                Return AddressOf SetLocationVisible
            Case "Command"
                Return AddressOf SetCommandVisible
            Case Else
                Throw New ArgumentException("Invalid element")
        End Select
    End Function

    Private Sub SetPanesVisible(visible As Boolean)
        SetPanesVisible(If(visible, "on", "disabled"))
    End Sub

    Private Sub SetLocationVisible(visible As Boolean)
        pnlLocation.Visible = visible
    End Sub

    Private Sub SetCommandVisible(visible As Boolean)
        pnlCommand.Visible = visible
    End Sub

    Private Sub StopGame()
        DoQuit()
    End Sub

    Private Sub ctlPlayerHtml_ShortcutKeyPressed(keys As System.Windows.Forms.Keys) Handles ctlPlayerHtml.ShortcutKeyPressed
        m_waiting = False
        txtCommand.Focus()
        RaiseEvent ShortcutKeyPressed(keys)
    End Sub

    Public Sub SetCompassDirections(dirs As IEnumerable(Of String)) Implements IPlayer.SetCompassDirections
        ctlCompass.CompassDirections = New List(Of String)(dirs)
        lstPlacesObjects.IgnoreNames = ctlCompass.CompassDirections
    End Sub

    Public Function GetURL(file As String) As String Implements IPlayer.GetURL
        Return file
    End Function

    Private Sub m_gameTimer_RequestNextTimerTick(nextTick As Integer) Handles m_gameTimer.RequestNextTimerTick
        If Not Me.IsHandleCreated Then Return
        BeginInvoke(Sub()
                        m_sendNextTickEventAfter = nextTick
                        m_tickCount = 0
                        tmrTick.Enabled = True
                    End Sub)
        ClearBuffer()
    End Sub

    Private Sub cmdFullScreen_Click(sender As System.Object, e As System.EventArgs) Handles cmdFullScreen.Click
        RaiseEvent ExitFullScreen()
        cmdFullScreen.Visible = False
    End Sub

    Public Sub ShowExitFullScreenButton()
        cmdFullScreen.Visible = True
    End Sub

    Public Sub DoPause(ms As Integer) Implements IPlayer.DoPause
        BeginInvoke(Sub()
                        m_pausing = True
                        tmrPause.Interval = ms
                        tmrPause.Enabled = True
                    End Sub)
    End Sub

    Private Sub tmrPause_Tick(sender As Object, e As System.EventArgs) Handles tmrPause.Tick
        tmrPause.Enabled = False
        m_pausing = False
        m_game.FinishPause()
        ClearBuffer()
    End Sub

    Private Sub m_walkthroughRunner_Output(text As String) Handles m_walkthroughRunner.Output
        BeginInvoke(Sub() m_htmlHelper.PrintText("<output>" + text + "</output>"))
    End Sub

    Public Property PostLaunchAction As Action
        Get
            Return m_postLaunchAction
        End Get
        Set(value As Action)
            m_postLaunchAction = value
        End Set
    End Property

    Public Property RecordWalkthrough As String
        Get
            Return m_recordWalkthrough
        End Get
        Set(value As String)
            m_recordWalkthrough = value
            If value IsNot Nothing Then
                m_recordedWalkthrough = New List(Of String)
            End If
        End Set
    End Property
End Class
