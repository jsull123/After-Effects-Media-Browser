   M S B A M L   `   `   `  ���� P  LVirtualizingWrapPanel, Version=2.1.0.0, Culture=neutral, PublicKeyToken=nullX@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanelWpfToolkit.Controls  H DMediaManager2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:MediaManager2MediaManager2    MediaManager2.MainWindow   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     Pvwp@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanel     -localclr-namespace:MediaManager2     -     � Title$	  smm��$	��750��$
��1325�����#232323 � WindowStartupLocation$ CenterScreen=� � 	Resources ,  &MediaManager2.ScaleMultiplierConverter   ScaleMultiplierConverter  MediaTemplate �� Orientation � ContextMenu �� ElementName �� Path �� 	Converter �� ConverterParameter  TextColor1Brush��%�  &	        &	      7   7   7    �� .R��� -   $ Vertical=� �� .@�m� -   $��Import��m� -   $��Open File Location��m� -   ���� *Name+.+��� -   ���� $ 
SizeSlider��$ Value�8   $	 256������ $ 
SizeSlider��$ Value�8  $	 144���� $��Center=�$��NoWrap=�$	��100��$��30��$��CharacterEllipsis=�$��Bottom=�8�� ���� *Name+.��� 	 � ColumnDefinitions	 �� $	��200 ��� $��* �
 � RowDefinitions
 �� $��50 ��� $��* �.+��� -     ButtonStyle2#��[ $��Select Folder��$
��Left=�$	��Top=�$��	10,15,0,0q�$��90��$��25��$��0��$��0���� -   #��[ $��Settings��$��Right=�$	��Top=�$��	0,15,10,0q�$��80��$��25��$��1��$��0���� -	   $��PlayPauseButton��$��20��$��20��$��Center=�$	��Top=�$��1��$��0��$��
0,15,200,0q�����  �� 
TargetType" ��.���  �� Property � �� �� Value
 ���� ��  � ��
 ���� ��  � �� ��  �� 
TargetType" ��.R�� #��z��#��z��.+��� 
������ F� $��PlayPauseIcon��O������� ��� � $� ��� � $� @x}р�� @x}� -1��� р����� р��@x}� -1@x}	  PrimaryColorBrush#��[  �� Stretch$ Uniform=�$��Center=�$��Center=� �� Triggers P�  P� Property �@J�IsMouseOver P� Value$
 True��. ���  �� 
TargetName$ PlayPauseIcon�� � �� �#a7cff7P�  � ��$
 True��. ��� $ PlayPauseIcon�� � ��
 ������� -
   $��
SizeSlider�� � Minimum$	 0.5Q� � Maximum$ 3Q� � Value$ 1Q� �� TickFrequency$	 0.1Q� �� IsSnapToTickEnabled$
 True��$��Center=�$	��Top=�$	��150��$��1��$��0��$��0,15,0,0q�� -    �Name$ CacheProgressBar��$��Center=�$	��Top=�$��3��$ 0Q�$	 100Q�$��0,20,0,0q�$��1��$��1�� K� ZIndex$ 1��$y�	Collapsed=�  StripedProgressBarStyle#��[ ���� $ FileListView��$ ActualWidth��� -    ��Name$ FileListCenterText��$��1��$��1��$��Center=�$��Center=�$ 1�� J� IsHitTestVisible$ False��  TextColor2Brush#��[ �� -    ��Name$ ActiveFolderButton��$��0��$��10,0,0,0q�$��12(�$��Active Folder��$
��Left=�$	��Top=�#��[ 
������ 
������ $��1���� -   $ SearchTextBoxPlaceholder��$��	Search...��$��0��$��1��$
��Left=�$ 1��$ False��#��[ $	��Top=�$��	17,24,0,0q��� -    ��Name$ SearchTextBox��$��1��$	��180��$��25��$��0,20,0,0q�$	��Top=�$��Center=�#��[   SecondaryColorBrush#��[ $��4q�$��0q��� -   $ ClearSearchButton��$��20��$��20��$��X��$��12(�$��Arial)�$��0��$��1��$��Right=�$	��Top=�
������ $��0q�$y�	Collapsed=�#��[ $��	0,23,14,0q����� " ��.���  � ��
 ���� ��  � ��
 �������  � ��
 �   ���  �@�Cursor$
 Hand�� �� Triggers P�  �@J�IsMouseOver$
 True��. ���  � ��
 ������� -   $ CacheProgressText��$	��Top=�$
��Left=�$��1��$��1��$��10,0,0,0q�#��[ R� -     R�Name$  FoldersTreeView��$��0��$��1��$��10,45,10,10q�  BackgroundColorBrush#��[ #��[ $��2,0,2,2q����� ! �� ItemsSource! �� *
Subfolders+.R��� #��[ ���� *Name+�� -   $��FileListView��$��1��$��1��$��10,20,10,10q�$��Disabled=�" �� SelectionMode$" Extended=�#��[ #��[ #��[ $	��2pxq����� .R�/   )WpfToolkit.Controls.VirtualizingWrapPanel  