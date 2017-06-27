package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.EditText;


public class addsupport extends Activity {

	String user = "";
	String pass = "";
	String supportSelect = "";
	String supportEmail = "";

	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.addsupport);

        Bundle extras = getIntent().getExtras();
        if (extras != null)
        {	    	    	
        	user = extras.getString("user");
        	pass = extras.getString("pass");
        	supportSelect = extras.getString("supportSelect");
        }
        Button addButton = (Button) findViewById(R.id.addButton);
        Button modifyButton = (Button) findViewById(R.id.modify);
        Button clearButton = (Button) findViewById(R.id.clearButton);
        EditText sName = (EditText) findViewById(R.id.sName);
        EditText sEmail = (EditText) findViewById(R.id.sEmail);
        TextView title = (TextView) findViewById(R.id.textView1);
        
        if (supportSelect!=null)
        {
        	title.setText("Modify Supporter");
        	sName.setText(supportSelect);
        	addButton.setVisibility(View.INVISIBLE);

        	SoapObject request = new SoapObject("http://tempuri.org/", "getSupportEmail");
       		request.addProperty("user", user);
       		request.addProperty("pass", pass);
       		request.addProperty("supportName", sName.getText().toString());
       		
   			SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
       		envelope.setOutputSoapObject(request);
       		envelope.dotNet = true;
             
       		try {
       			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
       			androidHttpTransport.call("http://tempuri.org/getSupportEmail", envelope);
       			SoapObject result = (SoapObject)envelope.bodyIn;
       			supportEmail = result.getPropertyAsString(0);
       		} catch (Exception e) {
       			e.printStackTrace();
       			Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
       		}
       		if (supportEmail.length()>0)
       			sEmail.setText(supportEmail);       			
        }
        else 
        	modifyButton.setVisibility(View.INVISIBLE);
        	
        addButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	
            	EditText sName = (EditText) findViewById(R.id.sName);
            	EditText sEmail = (EditText) findViewById(R.id.sEmail);
            	
        		SoapObject request = new SoapObject("http://tempuri.org/", "createSupporter");
	       		request.addProperty("user", user);
	       		request.addProperty("pass", pass);
	       		request.addProperty("supportName", sName.getText().toString());
	       		request.addProperty("supportEmail", sEmail.getText().toString());
	             
	       		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
	       		envelope.setOutputSoapObject(request);
	       		envelope.dotNet = true;
	             
	       		try {
	       			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
	       			androidHttpTransport.call("http://tempuri.org/createSupporter", envelope);
	       			SoapObject result = (SoapObject)envelope.bodyIn;
	       			Toast.makeText(getApplicationContext(), result.getPropertyAsString(0), Toast.LENGTH_LONG).show();
	       		} catch (Exception e) {
	       			e.printStackTrace();
	       			Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
	       		}
	       		finish();
            }     
        });  
        
        modifyButton.setOnClickListener(new View.OnClickListener() {
        	public void onClick(View view){
            	EditText sName = (EditText) findViewById(R.id.sName);
            	EditText sEmail = (EditText) findViewById(R.id.sEmail);

            	SoapObject request = new SoapObject("http://tempuri.org/", "updateSupport");
           		request.addProperty("user", user);
           		request.addProperty("pass", pass);
           		request.addProperty("oldSupportName", supportSelect);
           		request.addProperty("newSupportName", sName.getText().toString());
           		request.addProperty("newSupportEmail", sEmail.getText().toString());
                 
           		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
           		envelope.setOutputSoapObject(request);
           		envelope.dotNet = true;
                 
           		try {
           			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
           			androidHttpTransport.call("http://tempuri.org/updateSupport", envelope);
           			SoapObject result = (SoapObject)envelope.bodyIn;
           			Toast.makeText(getApplicationContext(), result.getPropertyAsString(0), Toast.LENGTH_LONG).show();
           		} catch (Exception e) {
           			e.printStackTrace();
           			Toast.makeText(getApplicationContext(), "in catch", Toast.LENGTH_LONG).show();
           		}
        		finish();
        	}
        });
        
        Button cancelButton = (Button) findViewById(R.id.cancelButton);
        cancelButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }
        });
        
        clearButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	EditText sName = (EditText) findViewById(R.id.sName);
            	EditText sEmail = (EditText) findViewById(R.id.sEmail);
            	sName.setText("");
            	sEmail.setText("");
            }
        });
        
	}
}
