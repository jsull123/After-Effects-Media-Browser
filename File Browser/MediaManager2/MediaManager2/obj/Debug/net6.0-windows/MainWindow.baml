   M S B A M L   `   `   `  ����P  LVirtualizingWrapPanel, Version=2.1.0.0, Culture=neutral, PublicKeyToken=nullX@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanelWpfToolkit.Controls  5   	   H DMediaManager2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:MediaManager2MediaManager2 5   	      MediaManager2.MainWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   Pvwp@clr-namespace:WpfToolkit.Controls;assembly=VirtualizingWrapPanel     5   	   -localclr-namespace:MediaManager2     5   	   -   5          � Title$	  smm��5	   	   $	��750��6   $
��1325��6"   ���#232323 � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5      ,  &MediaManager2.ScaleMultiplierConverter   ScaleMultiplierConverter  MediaTemplate �� Orientation � ContextMenu �� ElementName �� Path �� 	Converter �� ConverterParameter  TextColor1Brush��%�  &	        &	      7   7   7    5   
   6)   �� 5   
   .R�5      �� -   5        $ Vertical=�5       5      �� 5      .@�5      m� -   5        $��Import��5   #   63   m� 5      -   5        $��Open File Location��5   #   6?   m� 5      -   5        ��5   #   �� *Name+6;   5      .+�5      �� -   5        ��5      �� $ 
SizeSlider��$ Value�8   $	 256����5      �� $ 
SizeSlider��$ Value�8  $	 144��5"      �� 5#      $��Center=�62   $��NoWrap=�6O   $	��100��6e   $��30��6q   $��CharacterEllipsis=�6~   $��Bottom=�6�   8�� ��6   �� *Name+6�   5$      5%      5'      .��5(      � 	 � ColumnDefinitions	 5)   
   �� 5*      $	��200 �6   �� 5+      $��* �6   
 � RowDefinitions
 5-   
   �� 5.      $��50 �6   �� 5/      $��* �6   .+�53   
   �� -   5          ButtonStyle2#��[ 53      $��Select Folder��67   $
��Left=�6O   $	��Top=�6j   $��	10,15,0,0q�6�   $��90��6�   $��25��6�   $��0��6�   $��0��6�   �� 56   
   -   5        #��[ 56      $��Settings��67   $��Right=�6J   $	��Top=�6f   $��	0,15,10,0q�6�   $��80��6�   $��25��6�   $��1��6�   $��0��6�   �� 59   
   -	   5        $��PlayPauseButton��59      $��20��6F   $��20��6Q   $��Center=�6]   $	��Top=�6z   $��1��6�   $��0��6�   $��
0,15,200,0q�6�   ��5:      �� 5;       �� 
TargetType" ��6   .�5=      ��  �� Property � �� �� Value
 ���� 63   �� 5>       � ��
 ���� 64   �� 5?       � �� 5@      �� 5A       �� 
TargetType" ��6.   .R�5B   "   � #��z��6'   #��z��6G   .+�5C   &   �� 
������ 6-   F� 5E   &   $��PlayPauseIcon��6+   O������� ��� � $� ��� � $� @x}р�� @x}� -1��� р����� р��@x}� -1@x}	  PrimaryColorBrush#��[ 5G   )    �� Stretch$ Uniform=�5H   )   $��Center=�5I   )   $��Center=�5J   )   5K   #    �� Triggers 5L   "   P� 5N   &    P� Property �@J�IsMouseOver P� Value$
 True��6L   . �5O   *   ��  �� 
TargetName$ PlayPauseIcon��61    � �� �#a7cff76\   5P   '   P� 5R   &    � ��$
 True��6J   . �5S   *   �� $ PlayPauseIcon��61    � ��
 �����6\   5T   '   5V      5X      5Y      5[      �� 5^   
   -
   5        $��
SizeSlider��5^       � Minimum$	 0.5Q�6#    � Maximum$ 3Q�61    � Value$ 1Q�6=    �� TickFrequency$	 0.1Q�6G    �� IsSnapToTickEnabled$
 True��6[   $��Center=�6v   $	��Top=�6�   $	��150��6�   $��1��6�   $��0��6�   $��0,15,0,0q�6�   � 5`   
   -   5         �Name$ CacheProgressBar��5`      $��Center=�5a      $	��Top=�6/   $��3��5b      $ 0Q�5c      $	 100Q�5d      $��0,20,0,0q�6    $��1��5e      $��1��5f       K� ZIndex$ 1��5g      $y�	Collapsed=�5h        StripedProgressBarStyle#��[ 5i      ��5j      �� 5k      $ FileListView��6   $ ActualWidth�65   5m      �� 5o      -   5         ��Name$ FileListCenterText��5p      $��1��5q      $��1��5r      $��Center=�5s      $��Center=�5t      $ 1��5u       J� IsHitTestVisible$ False��5v        TextColor2Brush#��[ 5w      �� 5{   
   -   5         ��Name$ ActiveFolderButton��5{      $��0��5|   	   $��10,0,0,0q�5}   	   $��12(�5~   	   $��Active Folder��5   	   $
��Left=�5�   	   $	��Top=�5�   	   #��[ 5�   	   
������ 
������ $��1��5�   *   �� 5�   
   -   5        $ SearchTextBoxPlaceholder��5�      $��	Search...��5�      $��0��5�      $��1��5�      $
��Left=�5�      $ 1��5�      $ False��5�      #��[ 5�      $	��Top=�5�      $��	17,24,0,0q�6%   �� 5�   
   -   5         ��Name$ SearchTextBox��5�      $��1��5�   	   $	��180��5�   	   $��25��5�   	   $��0,20,0,0q�5�   	   $	��Top=�5�   	   $��Center=�5�   	   #��[ 5�   	     SecondaryColorBrush#��[ 5�   	   $��4q�5�   	   $��0q�5�   	   �� 5�   
   -   5        $ ClearSearchButton��5�      $��20��5�      $��20��5�      $��X��5�      $��12(�5�      $��Arial)�5�      $��0��5�      $��1��5�      $��Right=�5�      $	��Top=�5�      
������ $��0q�5�      $y�	Collapsed=�5�      #��[ 5�      $��	0,23,14,0q�5�      ��5�      �� 5�      " ��6   .�5�      ��  � ��
 ���� 63   �� 5�       � ��
 �����64   �� 5�       � ��
 �   �63   �� 5�       �@�Cursor$
 Hand��6/    �� Triggers 5�      P� 5�       �@J�IsMouseOver$
 True��69   . �5�      ��  � ��
 �����6;   5�      5�      5�      �� 5�   
   -   5        $ CacheProgressText��5�   	   $	��Top=�5�   	   $
��Left=�5�   	   $��1��5�   	   $��1��5�   	   $��10,0,0,0q�5�   	   #��[ 5�   	   R� 5�   
   -   5          R�Name$  FoldersTreeView��5�      $��0��5�      $��1��5�      $��10,45,10,10q�5�        BackgroundColorBrush#��[ 5�      #��[ 5�      $��2,0,2,2q�5�      ��5�      �� 5�      ! �� ItemsSource! 6+   �� *
Subfolders+.R�5�      �� #��[ 66   ��6    �� *Name+66   5�      5�      �� 5�   
   -   5        $��FileListView��5�      $��1��6'   $��1��67   $��10,20,10,10q�6D   $��Disabled=�5�      " �� SelectionMode$" Extended=�6I   #��[ 5�      #��[ 5�      #��[ 5�      $	��2pxq�5�      ��5�      �� 5�      .R�5�      /   )WpfToolkit.Controls.VirtualizingWrapPanel  5�      5�      5�      5�      