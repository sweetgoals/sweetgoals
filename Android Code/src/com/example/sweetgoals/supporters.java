package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;
import android.widget.EditText;
import android.widget.AdapterView.OnItemClickListener;


public class supporters extends Activity{
   
	String user = "";
	String pass = "";
	ListView lv;
	String[] supportList;
	ArrayAdapter lvAdaptor = null;

	
	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.supporters);

        Bundle extras = getIntent().getExtras();
        if (extras != null)
        {	    	    	
        	user = extras.getString("user");
        	pass = extras.getString("pass");
        }
        displaySupporters();
        Button addSupport = (Button) findViewById(R.id.addSupport);
        Button closeButton = (Button) findViewById(R.id.closeButton);
        addSupport.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	Intent myIntent = new Intent(view.getContext(), addsupport.class);
            	myIntent.putExtra("user", user);
            	myIntent.putExtra("pass", pass);
            	startActivityForResult(myIntent, 0);
            }     
        });    

        closeButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }
        });

	}
    
	@Override
    public void onResume()
    {      
    	super.onResume();
    	displaySupporters();
    }
	
	public void displaySupporters()
	{

		SoapObject request = new SoapObject("http://tempuri.org/", "listSupporters");       
        String dataString = "";
        request.addProperty("user", user);
        request.addProperty("pass", pass);
        
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/listSupporters", envelope);
    		SoapObject result = (SoapObject)envelope.bodyIn;
    		dataString = result.getPropertyAsString(0);
        } catch (Exception e) {
        	e.printStackTrace();
        	Toast.makeText(getApplicationContext(), "Database Call Failed", Toast.LENGTH_LONG).show();
        }

        supportList = dataString.split("___");

    	lvAdaptor = null;
    	lv = (ListView) findViewById(R.id.supportList);
    	lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, supportList);
    	lv.setAdapter(lvAdaptor);
    	lv.setTextFilterEnabled(true);           
    	//lvAdaptor.notifyDataSetChanged();
    	lv.setOnItemClickListener(new OnItemClickListener()
    	{
    		public void onItemClick(AdapterView<?> arg0, View v, int position, long id)
    		{
            	Intent myIntent = new Intent(v.getContext(), addsupport.class);
    			myIntent.putExtra("supportSelect", lv.getItemAtPosition(position).toString());
            	myIntent.putExtra("user", user);
            	myIntent.putExtra("pass", pass);
            	startActivityForResult(myIntent, 0);
    		}
    	});

	}
}
