package com.example.sweetgoals;


import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.text.SimpleDateFormat;
import java.util.Date;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import com.example.sweetgoals.creategoalsimple.saveClick;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.EditText;
import android.widget.Button;
import android.widget.Toast;

public class actstart extends Activity implements OnClickListener{
	String user = "";
	String pass= "";
	int goalNumber = 0;
	int actSeq = 0;
	String theDate = "";
	String time = "";
	String goalTitle = "";
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.actstart);

		Button startAct = (Button) findViewById(R.id.actStart);
		startAct.setOnClickListener(this);
		Button cancelButton = (Button) findViewById(R.id.actCancel);

		Bundle extras = getIntent().getExtras();
        user = extras.getString("user");
        pass = extras.getString("pass");
        goalTitle = extras.getString("gTitle");
        goalNumber = Integer.parseInt(extras.getString("gNumber"));
        actSeq = extras.getInt("actSeq");

	    SimpleDateFormat sdf = new SimpleDateFormat("MM/dd/yyyy");
	    theDate = sdf.format(new Date());
	    
	    sdf = new SimpleDateFormat("HH:mm");
	    time = sdf.format(new Date());
	    
        cancelButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }     
        });      
	}

    public void onClick(View v) {
		EditText actTitle = (EditText) findViewById(R.id.titleText);
		EditText actDesc = (EditText) findViewById(R.id.actDesc);
		
		SoapObject request = new SoapObject("http://tempuri.org/", "writeActivity");
		request.addProperty("userName", user);
		request.addProperty("password", pass);
		request.addProperty("goalNumber", goalNumber);
		request.addProperty("actTitle", actTitle.getText().toString());
		request.addProperty("adesc", actDesc.getText().toString());
		request.addProperty("startTime", time);
		request.addProperty("actDate", theDate);       
      
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
		envelope.setOutputSoapObject(request);
		envelope.dotNet = true;
      		
		try {
			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
			androidHttpTransport.call("http://tempuri.org/writeActivity", envelope);
			SoapObject result = (SoapObject)envelope.bodyIn;
			//Toast.makeText(getApplicationContext(), "wrote", Toast.LENGTH_LONG).show();
		} catch (Exception e) {
			e.printStackTrace();
			Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
		}
		finish();
    }
}
