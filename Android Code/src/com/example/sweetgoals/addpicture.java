package com.example.sweetgoals;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import java.net.URL;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.content.res.AssetManager;
import android.graphics.Bitmap;
import android.graphics.Bitmap.CompressFormat;
import android.graphics.BitmapFactory;
import android.graphics.Camera;
import android.graphics.drawable.Drawable;
import android.hardware.Camera.Parameters;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Base64;
import android.util.Log;
import android.util.Xml;
import android.view.View;
import android.widget.Button;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.MarshalBase64;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;
import org.xmlpull.v1.XmlSerializer;

public class addpicture extends Activity 
{
	protected Button _button;
	protected ImageView _image;
	protected TextView _field;
	protected String _path;
	protected boolean _taken;
	
	protected static final String PHOTO_TAKEN	= "photo_taken";
		
    @Override
    public void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.pictures);
       
        _image = ( ImageView ) findViewById( R.id.image);
        _field = ( TextView ) findViewById( R.id.field);
        _button = ( Button ) findViewById( R.id.button);
        _button.setOnClickListener( new ButtonClickHandler() );
        _path = Environment.getExternalStorageDirectory() + "/images/make_machine_example.jpg";
    }
    
    public class ButtonClickHandler implements View.OnClickListener 
    {
    	public void onClick( View view ){
    		Log.i("MakeMachine", "ButtonClickHandler.onClick()" );   		
    		startCameraActivity();
    	}
    }
    
    protected void startCameraActivity()
    {
    	Log.i("MakeMachine", "startCameraActivity()" );
    	File file = new File( _path );
    	Uri outputFileUri = Uri.fromFile( file );
    	Intent intent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE );
    	intent.putExtra( MediaStore.EXTRA_OUTPUT, outputFileUri );
    	intent.putExtra( MediaStore.EXTRA_SCREEN_ORIENTATION, ActivityInfo.SCREEN_ORIENTATION_PORTRAIT);    	
    	startActivityForResult( intent, 0 );
    }
    
    @Override
    protected void onActivityResult(int requestCode, int resultCode, Intent data) 
    {	
    	Log.i( "MakeMachine", "resultCode: " + resultCode );
    	switch( resultCode )
    	{
    		case 0:
    			Log.i( "MakeMachine", "User cancelled" );
    			break;
    			
    		case -1:
    			onPhotoTaken();
    			break;
    	}
    }
    
    protected void onPhotoTaken()
    {
    	Log.i( "MakeMachine", "onPhotoTaken" );
    	
    	_taken = true;
    	BitmapFactory.Options options = new BitmapFactory.Options();
        options.inSampleSize = 4;
    	Bitmap bitmap = BitmapFactory.decodeFile( _path, options );
    	_image.setImageBitmap(bitmap);
   	    
    	_field.setVisibility( View.GONE );
    	InputStream is = null;
    	byte[] bytearray = null;
        try
        {
        	is = new FileInputStream(_path);
        	if(_path != null)
        		try{
        			bytearray = streamToBytes(is);       			
        		}finally{
        			is.close();
        		}
        }catch (Exception e)
        {}

        SoapObject request = new SoapObject("http://tempuri.org/", "sendImage");        
   		request.addProperty("user", "dash");
   		request.addProperty("pass", "blah");
   		request.addProperty("actNumber", 5);
   		request.addProperty("myImage", bytearray);

   		SoapSerializationEnvelope envelope=new SoapSerializationEnvelope(SoapEnvelope.VER11);
   		new MarshalBase64().register(envelope);
        envelope.dotNet = true; 
        envelope.setOutputSoapObject(request);
        
   		try {
//   			Toast.makeText(getApplicationContext(), "Sending Pic", Toast.LENGTH_LONG).show();
//   			Toast.makeText(getApplicationContext(), "array length=" + bytearray.length, Toast.LENGTH_LONG).show();
   			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.letstrend.com/spursService.asmx?WSDL");
   			androidHttpTransport.call("http://tempuri.org/sendImage", envelope);
   			SoapObject result = (SoapObject)envelope.bodyIn;
   		} catch (Exception e) {
   			e.printStackTrace();   			
   			Toast.makeText(getApplicationContext(), "in catch e=" + e.getMessage(), Toast.LENGTH_LONG).show();
   			Toast.makeText(getApplicationContext(), "fault=" + ((SoapFault) envelope.bodyIn).faultstring, Toast.LENGTH_LONG).show();
   		}
    }

    public static byte[] streamToBytes(InputStream is) {
    	ByteArrayOutputStream os = new ByteArrayOutputStream(1024);
    	byte[] buffer = new byte[1024];
    	int len;
    	try {
    		while ((len = is.read(buffer)) >= 0) {
    			os.write(buffer, 0, len);
    		}
    	} catch (java.io.IOException e) {
    	}
    	return os.toByteArray();
    }
    
    @Override 
    protected void onRestoreInstanceState( Bundle savedInstanceState){
    	Log.i( "MakeMachine", "onRestoreInstanceState()");
    	if( savedInstanceState.getBoolean( addpicture.PHOTO_TAKEN ) ) {
    		onPhotoTaken();
    	}
    }
    
    @Override
    protected void onSaveInstanceState( Bundle outState ) {
    	outState.putBoolean( addpicture.PHOTO_TAKEN, _taken );
    }
}