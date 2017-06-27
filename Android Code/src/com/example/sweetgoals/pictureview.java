package com.example.sweetgoals;

import java.io.BufferedInputStream;
import java.io.BufferedOutputStream;
import java.io.ByteArrayOutputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.net.URL;

import android.app.Activity;
import android.content.Intent;
import android.content.pm.ActivityInfo;
import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.net.Uri;
import android.os.AsyncTask;
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

import com.example.sweetgoals.pictures.cancelButtonClick;

public class pictureview extends Activity {
    String user, pass, picNum, picLoc;

    public void onCreate(Bundle savedInstanceState) 
    {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.pictureview);
        user = "";
        pass = "";
        picNum = "";
        picLoc = "";
        Button closeButton;
        
        Bundle extras = getIntent().getExtras();
	    if (extras != null)
	    {	    	    	
	    	user = extras.getString("user");
	    	pass = extras.getString("pass");
	    	picNum = extras.getString("picNumber");
	    }
        closeButton = (Button) findViewById(R.id.closeButton);
        closeButton.setOnClickListener(new closeButtonClick());

        SoapObject request = new SoapObject("http://tempuri.org/", "getPicture");
		request.addProperty("user", user);
		request.addProperty("pass", pass);
		request.addProperty("picNumber", picNum);	
		//Toast.makeText(getApplicationContext(),  "picNumber= " + picNum, Toast.LENGTH_LONG).show();
    	
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
		envelope.setOutputSoapObject(request);
		envelope.dotNet = true;
		try {
			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
			androidHttpTransport.call("http://tempuri.org/getPicture", envelope);
			SoapObject result = (SoapObject)envelope.bodyIn;
			picLoc = result.getPropertyAsString(0);
//			Toast.makeText(getApplicationContext(),  "picLoc= " + picLoc, Toast.LENGTH_LONG).show();
			new DownloadImageTask((ImageView) findViewById(R.id.imageView1)).execute(picLoc);
			
		} catch (Exception e) {
			e.printStackTrace();
			String str= ((SoapFault) envelope.bodyIn).faultstring;		
			Toast.makeText(getApplicationContext(),  str, Toast.LENGTH_LONG).show();
			Log.d("soap error", "str " + str);
		}
    }
    
    public class closeButtonClick implements View.OnClickListener 
    {
    	public void onClick( View view ){
    		finish();
    	}
    }

    private class DownloadImageTask extends AsyncTask<String, Void, Bitmap> {
        ImageView bmImage;

        public DownloadImageTask(ImageView bmImage) {
            this.bmImage = bmImage;
        }

        protected Bitmap doInBackground(String... urls) {
            String urldisplay = urls[0];
            Bitmap mIcon11 = null;
            try {
                InputStream in = new java.net.URL(urldisplay).openStream();
                mIcon11 = BitmapFactory.decodeStream(in);
            } catch (Exception e) {
                Log.e("Error", e.getMessage());
                e.printStackTrace();
            }
            return mIcon11;
        }

        protected void onPostExecute(Bitmap result) {
            bmImage.setImageBitmap(result);
            
        }
    }
}

