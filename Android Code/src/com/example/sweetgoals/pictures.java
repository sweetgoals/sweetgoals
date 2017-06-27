package com.example.sweetgoals;

import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStream;
import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.Bundle;
import android.os.Environment;
import android.provider.MediaStore;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageView;
import android.widget.TextView;
import android.widget.Toast;
import android.util.Log;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.MarshalBase64;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

public class pictures extends Activity 
{
	protected Button photoButton, saveButton, cancelButton;
	protected ImageView photoTaken;
	protected TextView _field;
	protected String _path;
	protected boolean _taken;
	protected static final String PHOTO_TAKEN	= "photo_taken";
    String user, pass, actNum;
		
    @Override
    public void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.pictures);
        user = "";
        pass = "";
        actNum = "";
        
        Bundle extras = getIntent().getExtras();
	    if (extras != null)
	    {	    	    	
	    	user = extras.getString("user");
	    	pass = extras.getString("pass");
	    	actNum = extras.getString("actNum");
//   			Toast.makeText(getApplicationContext(), "user= " + user, Toast.LENGTH_LONG).show();
//   			Toast.makeText(getApplicationContext(), "pass= " + pass, Toast.LENGTH_LONG).show();
//   			Toast.makeText(getApplicationContext(), "actNum= " + actNum, Toast.LENGTH_LONG).show();
	    }

        photoTaken = ( ImageView ) findViewById( R.id.image);
        _field = ( TextView ) findViewById( R.id.field);
        _path = Environment.getExternalStorageDirectory() + "/images/make_machine_example.jpg";
        File file = new File(_path);
        file.delete();

        photoButton = ( Button ) findViewById( R.id.photoButton);
        photoButton.setOnClickListener( new ButtonClickHandler());
        saveButton = (Button) findViewById(R.id.saveButton);
        saveButton.setOnClickListener(new saveButtonClick());
        cancelButton = (Button) findViewById(R.id.cancelButton);
        cancelButton.setOnClickListener(new cancelButtonClick());
    }
    
    public class ButtonClickHandler implements View.OnClickListener 
    {
    	public void onClick( View view ){
    		Log.i("MakeMachine", "ButtonClickHandler.onClick()" );   		
    		startCameraActivity();
    	}
    }

    public class cancelButtonClick implements View.OnClickListener 
    {
    	public void onClick( View view ){
    		finish();
    	}
    }

    public class saveButtonClick implements View.OnClickListener 
    {
    	public void onClick( View view ){
        	InputStream is = null;
        	byte[] bytearray = null;

        	EditText picDesc =(EditText) findViewById( R.id.picDescText);
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
            {
            	Log.d("path error", "path error");
            }
//            if (bytearray==null)
//            	Toast.makeText(getApplicationContext(), "bytearray = null", Toast.LENGTH_LONG).show();
//            else
//            	Toast.makeText(getApplicationContext(), "bytearray length= " + bytearray.length, Toast.LENGTH_LONG).show();
            if (bytearray != null)
            {
//		        Toast.makeText(getApplicationContext(), "user=" + user, Toast.LENGTH_LONG).show();
//		        Toast.makeText(getApplicationContext(), "pass=" +pass, Toast.LENGTH_LONG).show();
//		        Toast.makeText(getApplicationContext(), "actNumber=" + actNum, Toast.LENGTH_LONG).show();
		        SoapObject request = new SoapObject("http://tempuri.org/", "sendImage");        
		   		request.addProperty("user", user);
		   		request.addProperty("pass", pass);
		   		request.addProperty("actNumber", actNum);
		   		request.addProperty("picDesc", picDesc.getText().toString());
		   		request.addProperty("myImage", bytearray);
		
		   		SoapSerializationEnvelope envelope=new SoapSerializationEnvelope(SoapEnvelope.VER11);
		   		new MarshalBase64().register(envelope);
		        envelope.dotNet = true; 
		        envelope.setOutputSoapObject(request);
		        
		   		try {
		   			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
		   			androidHttpTransport.call("http://tempuri.org/sendImage", envelope);
		   			SoapObject result = (SoapObject)envelope.bodyIn;
		   			Toast.makeText(getApplicationContext(), "Saved", Toast.LENGTH_LONG).show();
		   		} catch (Exception e) {
		   			e.printStackTrace();   			
		   			Toast.makeText(getApplicationContext(), "in catch e=" + e.getMessage(), Toast.LENGTH_LONG).show();
		   			Toast.makeText(getApplicationContext(), "fault=" + ((SoapFault) envelope.bodyIn).faultstring, Toast.LENGTH_LONG).show();
		   		}
		    	finish();
            }
            else
            	Toast.makeText(getApplicationContext(), "Need to Take Picture", Toast.LENGTH_LONG).show();	
    	}

    }

    protected void startCameraActivity()
    {
    	Log.i("MakeMachine", "startCameraActivity()" );
    	File file = new File( _path );
    	Uri outputFileUri = Uri.fromFile( file );
    	Intent intent = new Intent(android.provider.MediaStore.ACTION_IMAGE_CAPTURE);
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
    	photoTaken.setImageBitmap(bitmap);
    	_field.setVisibility( View.GONE );
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
    	if( savedInstanceState.getBoolean( pictures.PHOTO_TAKEN ) ) {
    		onPhotoTaken();
    	}
    }
    
    @Override
    protected void onSaveInstanceState( Bundle outState ) {
    	outState.putBoolean( pictures.PHOTO_TAKEN, _taken );
    }
}