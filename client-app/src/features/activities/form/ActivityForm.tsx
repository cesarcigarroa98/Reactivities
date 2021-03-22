import { observer } from 'mobx-react-lite';
import React, { ChangeEvent, useState } from 'react';
import {Segment, Form, Button} from 'semantic-ui-react';
import { Activity } from '../../../app/models/activity';
import { useStore } from '../../../app/stores/store';

export default observer(function ActivityForm () {
    const {activityStore} = useStore();
    const {selectedActivity, closeForm, createActivity, updateActivity, loading} = activityStore;

    const initialState = selectedActivity ?? {
        id: '',
        title: '',
        category: '',
        description: '',
        date: '',
        city: '',
        venue: ''
    }

    const [activity, setActivity] = useState<Activity>(initialState);

    function handleSubmit(){
        activity.id ? updateActivity(activity) : createActivity(activity);
    }

    {/*This function assigns the input's values to the object's properties*/}
    function handleInputChange(event: ChangeEvent<HTMLInputElement | HTMLTextAreaElement>){
        {/*name=name of the input. value=current input's value*/}
        const {name, value} = event.target;
        {/*"...activity" destructures the object's properties so they can be accessed one by one*/}
        setActivity({...activity, [name]: value});
    }

    return (
        <Segment clearing>
            <Form onSubmit={handleSubmit} autoComplete='off'>
                <Form.Input placeholder='Title' value={activity.title} name='title' onChange={handleInputChange}></Form.Input>
                <Form.TextArea placeholder='Description' value={activity.description} name='description' onChange={handleInputChange}></Form.TextArea>
                <Form.Input placeholder='Category' value={activity.category} name='category' onChange={handleInputChange}></Form.Input>
                <Form.Input type='date' placeholder='Date' value={activity.date} name='date' onChange={handleInputChange}></Form.Input>
                <Form.Input placeholder='City' value={activity.city} name='city' onChange={handleInputChange}></Form.Input>
                <Form.Input placeholder='Venue' value={activity.venue} name='venue' onChange={handleInputChange}></Form.Input>
                <Button loading={loading} floated='right' positive type='submit' content='Submit'></Button>
                <Button onClick={closeForm} floated='right' type='button' content='Cancel'></Button>
            </Form>
        </Segment>
    )
})