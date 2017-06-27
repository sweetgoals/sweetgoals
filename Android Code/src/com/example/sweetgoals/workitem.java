package com.example.sweetgoals;
import java.io.BufferedReader;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;

import org.ksoap2.SoapEnvelope;
import org.ksoap2.serialization.SoapObject;
import org.ksoap2.serialization.SoapSerializationEnvelope;
import org.ksoap2.transport.HttpTransportSE;

import com.example.sweetgoals.R;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.Spinner;
import android.widget.Toast;
import android.widget.AdapterView.OnItemClickListener;

public class workitem extends Activity {

	@Override
	public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.actstart);

        Button cancelButton = (Button) findViewById(R.id.cancelButton);
        
        EditText workTitle = (EditText) findViewById(R.id.titleText);
        EditText timeLength = (EditText) findViewById(R.id.timeLength);
        EditText description = (EditText) findViewById(R.id.goalDesc);
        Spinner timeUnit = (Spinner) findViewById(R.id.timeUnit);


        cancelButton.setOnClickListener(new View.OnClickListener() {
            public void onClick(View view) {
            	finish();
            }     
        });      
	}

	
	
}
