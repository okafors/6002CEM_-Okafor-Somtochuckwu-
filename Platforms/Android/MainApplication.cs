﻿using Android.App;
using Android.Runtime;
using Firebase;

namespace TodoApp;

[Application]
public class MainApplication : MauiApplication
{
	public MainApplication(IntPtr handle, JniHandleOwnership ownership)
		: base(handle, ownership)
	{
	}
    public override void OnCreate()
    {
        base.OnCreate();
        FirebaseApp.InitializeApp(this);
    }

    protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
}
