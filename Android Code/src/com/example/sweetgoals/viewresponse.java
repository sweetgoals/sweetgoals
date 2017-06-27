package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.ContentProviderOperation.Builder;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;
import android.widget.AdapterView.OnItemClickListener;

public class viewresponse extends Activity{
	
	ArrayAdapter lvAdaptor = null;
	ListView lv;
	String responses = "";
	String supporters = "";
	String user, pass, aNumber;
	String responseArr[], supportersArr[];
	
	@Override
	public void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
        setContentView(R.layout.viewresponse);
		Button closeButton = (Button) findViewById(R.id.closeButton);

		Bundle extras = getIntent().getExtras();
	    if (extras != null)
	    {
	     aNumber = extras.getString("actNumber");
	     user = extras.getString("user");
	     pass = extras.getString("pass");
	    }
//        aNumber = "29";
//		Toast.makeText(getApplicationContext(),  "user=" + user, Toast.LENGTH_LONG).show();
//		Toast.makeText(getApplicationContext(),  "pass=" + pass, Toast.LENGTH_LONG).show();
//		Toast.makeText(getApplicationContext(),  "actnum=" + aNumber, Toast.LENGTH_LONG).show();

        SoapObject request = new SoapObject("http://tempuri.org/", "getSupportResponses");
		request.addProperty("user", user);
		request.addProperty("pass", pass);
		request.addProperty("actNum", aNumber);	
    	
		SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
		envelope.setOutputSoapObject(request);
		envelope.dotNet = true;
		try {
			HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
			androidHttpTransport.call("http://tempuri.org/getSupportResponses", envelope);
			SoapObject result = (SoapObject)envelope.bodyIn;
			//Log.d("result=", result.
			
			supporters = result.getPropertyAsString(0);
			responses = result.getPropertyAsString(1);
			//Toast.makeText(getApplicationContext(),  "results=" + result.getPropertyAsString(0), Toast.LENGTH_LONG).show();
			if (!supporters.contains("anyType{}"))
			{
				supportersArr = result.getPropertyAsString("supports").split("___"); 
				responseArr = result.getPropertyAsString("responses").split("___");
			}
			else			
			{
				supportersArr = new String[1];
				supportersArr[0] = "Not Confirmed Yet";
				responseArr = new String[1];
				responseArr[0] = "No Note";

			}
			
		} catch (Exception e) {
			e.printStackTrace();
			String str= ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  str, Toast.LENGTH_LONG).show();
			Log.d("soap error", "str " + str);
		}
//		Toast.makeText(getApplicationContext(),  "supporters=" + supporters, Toast.LENGTH_LONG).show();

		lv = (ListView) findViewById(R.id.responseList);
        lv.setAdapter(null);        
        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, supportersArr);            
        lv.setAdapter(lvAdaptor);
        lv.setTextFilterEnabled(true);           
        lvAdaptor.notifyDataSetChanged();
    	lv.setOnItemClickListener(new OnItemClickListener() {
    		public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
    		{
    			new AlertDialog.Builder(viewresponse.this)
    		    .setTitle(responseArr[position])
    		    .setPositiveButton("OK", null)
    		    .show();
    		}
    	});               	
    	
        closeButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }     
        });      

	}


}
