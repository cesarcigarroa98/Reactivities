import React, { Fragment} from 'react';
import { Container} from 'semantic-ui-react';
import NavBar from './NavBar';
import ActivityDashboard from '../../features/activities/dashboard/ActivityDashboard';
import { observer } from 'mobx-react-lite';
import { Route, Switch, useLocation } from 'react-router';
import HomePage from '../../features/home/HomePage';
import ActivityForm from '../../features/activities/form/ActivityForm';
import ActivityDetails from '../../features/activities/details/ActivityDetails';
import TestError from '../../features/errors/TestError';
import { ToastContainer } from 'react-toastify';
import NotFound from '../../features/errors/NotFound';
import ServerError from '../../features/errors/ServerError';

function App() {
  const location = useLocation();

  /*exact is used to avoid calling a component if paths match somehow */

  /*location.key is an unique value created by component when it is mounted.
   Key property is making a new component to be mounted every time its assigned value is changed */
   /*We make this because if you are inside edit form and then click on new form, component with edit data must disappear and a
   new one must be created*/

   /*Switch makes each route exclusive. This means only one component can be loaded if route matches */

   /*Component not foud is rendered when anyothe route is reached */

  return (
    <>
      <ToastContainer position='bottom-right' hideProgressBar/>
      <Route exact path='/' component={HomePage} />
      <Route 
        /*Any route that matches "/" + something else*/
        path={'/(.+)'}
        /*Render method */
        render={() => (
          <>
            <NavBar />
            <Container style={{marginTop: '7em'}}>
              <Switch>
                <Route exact path='/activities' component={ActivityDashboard} />
                <Route path='/activities/:id' component={ActivityDetails} />
                <Route key={location.key} path={['/createActivity', '/manage/:id']} component={ActivityForm} />
                <Route path='/errors' component={TestError}/>
                <Route path='/server-error' component={ServerError} />
                <Route component={NotFound} />
              </Switch>
            </Container>
          </>
        )}
      />
    </>
  );
}

export default observer(App);
