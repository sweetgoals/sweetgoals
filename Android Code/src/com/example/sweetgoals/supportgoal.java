package com.example.sweetgoals;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.SoapFault;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.ListView;
import android.widget.Toast;
import java.util.ArrayList;
import java.util.List;

public class supportgoal extends Activity {

	protected String[] supporters = null; // { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };
	protected boolean[] supSelect = null; // =  new boolean[ _options.length ];
	ListView lv;
	
	String user = "";
	String pass = "";
	String goalTitle = "";
	Button addButton;
	
    @Override
	public void onCreate(Bundle savedInstanceState) {
    	super.onCreate(savedInstanceState);
        setContentView(R.layout.supportgoal);
        addButton = (Button) findViewById(R.id.addButton);
        addButton.setOnClickListener( new addButtonClick());

        Bundle extras = getIntent().getExtras();
        user = extras.getString("user");
        pass = extras.getString("pass");
        goalTitle = extras.getString("gTitle");
        
        Button finishButton = (Button) findViewById(R.id.finishButton);
        finishButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	updateGoalSupport();
            	finish();
            }
        });

        Button cancelButton = (Button) findViewById(R.id.cancelButton);
        cancelButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }
        });
        loadSupportList();
    }

    public class addButtonClick implements View.OnClickListener {
		public void onClick( View view ) {
			showDialog( 0 );
		}
	}

    public void loadSupportList()
    {
    	String dataString = "";
    	ArrayAdapter lvAdaptor = null;
        SoapObject request = new SoapObject("http://tempuri.org/", "listGoalSupporters");       
        request.addProperty("user", user);
        request.addProperty("pass", pass);
        request.addProperty("goalTitle", goalTitle);
        
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/listGoalSupporters", envelope);
    		SoapObject result = (SoapObject)envelope.bodyIn;
    		dataString = result.getPropertyAsString(0);
        } catch (Exception e) {
			e.printStackTrace();
			String error = ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
			Log.d("soap error", "listGoalSupporters error= " + error);
        }
        
        supporters = dataString.split("___");
        lv = (ListView) findViewById(R.id.supList);
        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, supporters);
        lv.setAdapter(lvAdaptor);
        lv.setTextFilterEnabled(true);
    }
    
    public void updateGoalSupport()
    {
    	String goalSup = "";
    	int i=0;
    	for(i=0;i<lv.getCount();i++)
    		goalSup += lv.getItemAtPosition(i) + "___";
   	
        SoapObject request = new SoapObject("http://tempuri.org/", "addGoalSupport");       
        request.addProperty("user", user);
        request.addProperty("pass", pass);
        request.addProperty("goalTitle", goalTitle);
        request.addProperty("goalSup", goalSup);
        
        
        SoapSerializationEnvelope envelope = new SoapSerializationEnvelope(SoapEnvelope.VER11);     
        envelope.setOutputSoapObject(request);
        envelope.dotNet = true;
        
        try {
        	HttpTransportSE androidHttpTransport = new HttpTransportSE("http://www.yobbers.com/spursService.asmx?WSDL");
        	androidHttpTransport.call("http://tempuri.org/addGoalSupport", envelope);
    		SoapObject result = (SoapObject)envelope.bodyIn;
        } catch (Exception e) {
			e.printStackTrace();
			String error = ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
			Log.d("soap error", "addGoalSupport error= " + error);
        }
    }

    @Override
	protected Dialog onCreateDialog( int id ) 
	{		      
        String dataString = "";

        SoapObject request = new SoapObject("http://tempuri.org/", "listSupporters");       
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
			String error = ((SoapFault) envelope.bodyIn).faultstring;
			Toast.makeText(getApplicationContext(),  error, Toast.LENGTH_LONG).show();
			Log.d("soap error", "listSupporters error= " + error);
        }
        supporters = dataString.split("___");
        supSelect = new boolean[ supporters.length ];

        return 				
		new AlertDialog.Builder( this )
        	.setTitle( "Select Supporters" )
        	.setMultiChoiceItems( supporters, supSelect, new DialogSelectionClickHandler() )
        	.setPositiveButton( "OK", new DialogButtonClickHandler() )
        	.create();
	}
	
	public class DialogSelectionClickHandler implements DialogInterface.OnMultiChoiceClickListener
	{
		public void onClick( DialogInterface dialog, int clicked, boolean selected )
		{
			printSelectedDays();
		}
	}

	public class DialogButtonClickHandler implements DialogInterface.OnClickListener
	{
		public void onClick( DialogInterface dialog, int clicked )
		{
			switch( clicked )
			{
				case DialogInterface.BUTTON_POSITIVE:
					printSelectedDays();
					break;
			}
		}
	}
	
	protected void printSelectedDays(){
		ArrayAdapter lvAdaptor = null;
		List<String> slist = new ArrayList<String>();
		
		for( int i = 0; i < supporters.length; i++ ){
			if (supSelect[i]==true)
				slist.add(supporters[i]);		
		}
		
        lv = (ListView) findViewById(R.id.supList);
        lvAdaptor = new ArrayAdapter<String>(this,android.R.layout.simple_list_item_1, slist);
        lv.setAdapter(lvAdaptor);
        lv.setTextFilterEnabled(true);
	}
}
