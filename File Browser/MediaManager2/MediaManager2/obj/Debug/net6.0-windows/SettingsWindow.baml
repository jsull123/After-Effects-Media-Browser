   M S B A M L   `   `   `  ����H  DMediaManager2, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null-clr-namespace:MediaManager2MediaManager2  5   	   "    MediaManager2.SettingsWindow   5   	   W SPresentationCore, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35e aSystem.Windows.Controls.Ribbon, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089R NWindowsBase, Version=6.0.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35F 9http://schemas.microsoft.com/winfx/2006/xaml/presentation     5   	   R NSystem.Xaml, Version=6.0.2.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   @d2http://schemas.microsoft.com/expression/blend/2008     5   	   Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     5   	   -localclr-namespace:MediaManager2     5   	   -   5        ���#232323  � Title$  Settings��5	   	   $	��450��6   $	��450��6'    � WindowStartupLocation$ CenterScreen=�5   	    � 	Resources 5      ��%    5      .��5      � .+�5   
   �� -   5           ButtonStyle1#��[  5      $��Clear Media Cache��67   $
��Left=�6S   $	��Top=�6n   $��	33,40,0,0q�6�   $	��110��6�   �� 5   
   -   5         ��Name$ CacheSizeTextBlock��5      $
��Left=�5      $	��Top=�5      
�������$	��...��5      $��
148,42,0,0q�6   � 5   
   -   5         �Name$ CacheProgressBar��5      $
��Left=�5      $	��Top=�6-   $��3��5       � Minimum$ 0Q�5       � Maximum$	 100Q�5       $��	33,65,0,0q�6    $	��384��63   $y�	Collapsed=�5!        StripedProgressBarStyle#��[ 5"      �� 5$   
   -   5        $ ScrollSpeedSettingLabel��5%      $	��Top=�5&      $
��Left=�5'      $��Scroll Speed��5(        TextColor1Brush#��[ 5)      $��	33,73,0,0q�6;   �� 5,   
   $
��Left=�5-      $	��Top=�5.        TextColor2Brush#��[ 5/      $��
267,73,0,0q�6B   ��5,      ��  �� ElementName$ ScrollSpeedSlider�� �� Path$ Value�	 �� StringFormat$	 Value: {0:F1}��5/   B   52      53      